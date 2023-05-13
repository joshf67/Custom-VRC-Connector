using Joshf67.ServerConnector.Packing;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Data;

namespace Joshf67.ServerConnector.Development.Tests
{

    /// <summary>
    /// A set of test functions to ensure underlying code is not broken
    /// </summary>
    public static class ServerConnectorTestSuit
    {
        /// <summary>
        /// Runs all of the tests
        /// </summary>
        /// <returns> Returns if any of the test failed </returns>
        [MenuItem("Server Connector/Run Tests")]
        public static bool RunTests()
        {
            return TestMessagePacker();
        }

        /// <summary>
        /// Log any test that failed to the console
        /// </summary>
        /// <param name="testName"> The test that failed </param>
        /// <param name="errors"> The errors with the test </param>
        private static void LogTestFail(string testName, DataList errors)
        {
            Debug.LogError($"MessagePacker failed test '{testName}'");
            for (int i = 0; i < errors.Count; i++)
            {
                Debug.LogError(errors[i].String);
            }
        }

        /// <summary>
        /// Compare the results and expected results of a message packing
        /// </summary>
        /// <param name="testName"> The name of the test </param>
        /// <param name="packedResults"> The packed results </param>
        /// <param name="expectedResults"> The expected results </param>
        /// <returns> Boolean if any test fails </returns>
        private static bool CompareMessagePackerResults(string testName, DataList packedResults, DataList expectedResults)
        {
            DataList errors = new DataList();

            if (expectedResults.Count != packedResults.Count)
            {
                errors.Add("Result Bytes are not the same as Expected");
            }

            //Loop through all the results and check they are the same as expected
            for (int resultIndex = 0; resultIndex < packedResults.Count; resultIndex++)
            {
                DataToken result;
                DataToken expected;

                if (packedResults.TryGetValue(resultIndex, out result))
                {
                    if (expectedResults.TryGetValue(resultIndex, out expected))
                    {
                        if (result.Int != expected.Int)
                            errors.Add($"Expected {expected.Int} but actually got {result.Int}");
                    } 
                    else
                    {
                        errors.Add($"Either packedResults returned more bytes or the test was" +
                            $"not setup properly as index {resultIndex} failed for expected");
                    }
                } 
                else
                {
                    errors.Add($"packedResults did not return a value for index {resultIndex}");
                }
            }

            if (errors.Count != 0)
            {
                LogTestFail(testName, errors);
                return false;
            }

            Debug.Log($"MessagePacker test succeeded '{testName}'");
            return true;
        }

        /// <summary>
        /// Run tests on the message packer
        /// </summary>
        /// <returns> If any of the tests failed </returns>
        private static bool TestMessagePacker()
        {
            DataList uncompressedList = new DataList();
            DataList expectedResults = new DataList();
            DataList packingInputs = new DataList();
            bool failedAnyTest = false;

            Debug.Log($"Starting MessagePacker tests");

            //Test simple Byte
            expectedResults.Clear();
            expectedResults.Add(1048832);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage((byte)1));

            if (CompareMessagePackerResults("Test simple Byte",
                MessagePacker.PackMessageBytesToURL(MessagePacker.CompressMessage((byte)1), 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Big endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1064960);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(1, 2));

            if (CompareMessagePackerResults("Test Big endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Big endian with more bits
            expectedResults.Clear();
            expectedResults.Add(1048768);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(1, 9));
            packingInputs.Add(MessagePacker.CompressMessage((byte)1, 1));

            if (CompareMessagePackerResults("Test Big endian with more bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Little endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1064960);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(1, 2, (int)PackingType.LittleEndian));

            if (CompareMessagePackerResults("Test Little endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Little endian with more bits
            expectedResults.Clear();
            expectedResults.Add(1050176);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(262, 10, (int)PackingType.LittleEndian));

            if (CompareMessagePackerResults("Test Little endian with more bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Little endian with more bits and Big Endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1050137);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(262, 12, (int)PackingType.LittleEndian));
            packingInputs.Add(MessagePacker.CompressMessage(9, 4));

            if (CompareMessagePackerResults("Test Little endian with more bits and Big Endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Big endian with more bits and Little Endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1052777);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(262, 12));
            packingInputs.Add(MessagePacker.CompressMessage(9, 4, (int)PackingType.LittleEndian));

            if (CompareMessagePackerResults("Test Big endian with more bits and Little Endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Sequential with less bits followed by Big Endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1048576);
            expectedResults.Add(16);
            expectedResults.Add(0);
            expectedResults.Add(36864);

            uncompressedList.Clear();
            uncompressedList.Add(1);
            uncompressedList.Add(8);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(uncompressedList, 62));
            packingInputs.Add(MessagePacker.CompressMessage((byte)1, 2));

            if (CompareMessagePackerResults("Test Sequential with less bits followed by Big Endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Sequential with more bits followed by Big Endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1048576);
            expectedResults.Add(16);
            expectedResults.Add(0);
            expectedResults.Add(33024);

            uncompressedList.Clear();
            uncompressedList.Add(1);
            uncompressedList.Add(8);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(uncompressedList, 66));
            packingInputs.Add(MessagePacker.CompressMessage((byte)1, 2));

            if (CompareMessagePackerResults("Test Sequential with more bits followed by Big Endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Sequential with more bits followed by Big Endian with more bits
            expectedResults.Clear();
            expectedResults.Add(1048576);
            expectedResults.Add(16);
            expectedResults.Add(0);
            expectedResults.Add(32768);
            expectedResults.Add(262144); 

            uncompressedList.Clear();
            uncompressedList.Add(1);
            uncompressedList.Add(8);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(uncompressedList, 66));
            packingInputs.Add(MessagePacker.CompressMessage(1, 12));

            if (CompareMessagePackerResults("Test Sequential with more bits followed by Big Endian with more bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Sequential with more bits followed by Little Endian with less bits
            expectedResults.Clear();
            expectedResults.Add(1048576);
            expectedResults.Add(16);
            expectedResults.Add(0);
            expectedResults.Add(33024);

            uncompressedList.Clear();
            uncompressedList.Add(1);
            uncompressedList.Add(8);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(uncompressedList, 66));
            packingInputs.Add(MessagePacker.CompressMessage(1, 2, (int)PackingType.LittleEndian));

            if (CompareMessagePackerResults("Test Sequential with more bits followed by Little Endian with less bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            //Test Sequential with more bits followed by Little Endian with more bits
            expectedResults.Clear();
            //expectedResults.Add(1050912);
            expectedResults.Add(1048576);
            expectedResults.Add(16);
            expectedResults.Add(0);
            expectedResults.Add(32804);
            expectedResults.Add(524288);

            uncompressedList.Clear();
            uncompressedList.Add(1);
            uncompressedList.Add(8);

            packingInputs.Clear();
            packingInputs.Add(MessagePacker.CompressMessage(uncompressedList, 66));
            packingInputs.Add(MessagePacker.CompressMessage(521, 12, (int)PackingType.LittleEndian));

            if (CompareMessagePackerResults("Test Sequential with more bits followed by Little Endian with more bits",
                MessagePacker.PackMessageBytesToURL(packingInputs, 0, 21, 4), expectedResults))
                failedAnyTest = true;



            return !failedAnyTest;
        }
    }
}
