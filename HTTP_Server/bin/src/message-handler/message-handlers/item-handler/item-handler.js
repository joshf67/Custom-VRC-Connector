var path = require("path");
const logger = require("../../../logger");
const UserConnectionData = require("../../../connection-handler/user-connection-data");
const URLMessage = require("../../url-message");
const ResponseHandler = require("../../../response-handler/response-handler");
const ResponseData = require("../../../response-handler/response-data");
const { ResponseTypes } = require("../../../response-handler/response-types");
const { MessageLength } = require("../../message-length");
const MessageBuilder = require("../../message-builder");
const {
  ItemSchemaJS,
} = require("../../../database-handler/database-schema/inventory/item-schema");

class ItemHandler {
  /**
   * Handles the first item related message and sets up a user's expecting data
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {String[]} message - The parsed message from the connection
   */
  static HandleInitialMessage(user, res, message) {
    //First bit of a Item message is use to determine if it should be adding or removing
    let addingItems = message[0] == "1";

    //The next 7 bits are used to store how many items/indices are in the message
    let items = parseInt(message.slice(1, 8).join(""), 2);

    //Build the message listener data
    user.expectingDataState = new MessageBuilder(
      ItemHandler.HandleMessageUpdate,
      ItemHandler.HandleMessageFinish,
      addingItems == true
        ? MessageLength.Item * items
        : MessageLength.ItemIndex * items,
      {
        addingItems: addingItems,
        items: items,
      }
    );

    //Setup the message listener and then send the remaining message bits into it
    user.expectingDataCallback = user.expectingDataState.AddMessageBytes;
    user.expectingDataCallback(user, res, message.slice(8));
  }

  /**
   * Handles item related messages
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {*} bitsRemaining - The bits remaining until the message is complete
   */
  static HandleMessageUpdate(user, res, bitsRemaining) {
    //Continue listening to data
    ResponseHandler.HandleResponse(
      user,
      res,
      new ResponseData(
        ResponseTypes.Item_Updated,
        `Item message Recieved, Awaiting ${bitsRemaining} bits`
      )
    );
  }

  /**
   * Handles finishing up item messages
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {string[]} fullMessageBits - The message in a string binary array
   * @param {*} options - The options passed to the message builder on creation
   */
  static HandleMessageFinish(user, res, fullMessageBits, options) {
    if (options.addingItems) {
      let items = [];

      //Build up all of the items to add from the message bits
      for (let i = 0; i < options.items * MessageLength.Item; i += MessageLength.Item) {
        items.push(
          new ItemSchemaJS(
            parseInt(fullMessageBits.slice(i, i + MessageLength.Item).join(""), 2)
          )
        );
      }

      logger.log(items);
      ItemHandler.AddItems(user, res, items);
    } else {
      let indices = [];

      //Build up all of the indices from the message bits
      for (let i = 0; i < options.items * MessageLength.Item; i += MessageLength.Item) {
        indices.push(
          parseInt(fullMessageBits.slice(i, i + MessageLength.ItemIndex).join(""), 2)
        );
      }

      logger.log(indices);
      ItemHandler.RemoveItems(user, res, indices);
    }
  }

  /**
   * Handles item addition
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {ItemSchemaJS[]} items - The items to be added
   */
  static AddItems(user, res, items) {
    //Update the local inventory items
    user.databaseData.Inventory.Items =
      user.databaseData?.Inventory?.Items?.concat?.(items);

    //Fire off a request to save the data into the database
    user.databaseData
      .save()
      .then((saved) => {
        //If it saved then respond with succeeded message
        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.Added_item, items)
        );
      })
      .catch((error) => {
        logger.warn(error);
        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.Server_Error)
        );
      });
  }

  /**
   * Handles item removal
   * @param {UserConnectionData} user - The user this message is for
   * @param {*} res
   * @param {int[]} indices - The item indexes to be removed
   */
  static RemoveItems(user, res, indices) {
    /* sort all of the indices by largets first so that when we remove
       any items it wont affect the rest of the indices position
       and remove any indices that are above the actual inventory item count */
    indices = indices.sort((a, b) => {
      return a - b;
    }).filter(
      (index) => index < (user.databaseData?.Inventory?.Items.length ?? 0)
    );

    //If there are no indices left then we don't need to message the database, so respond
    if (indices.length == 0) {
      return ResponseHandler.HandleResponse(
        user,
        res,
        new ResponseData(ResponseTypes.Removed_item, indices)
      );
    }  

    //Remove all of the incides
    indices.forEach((index) => {
      user.databaseData?.Inventory?.Items?.splice?.(index, 1);
    });

    //Fire off a request to save the data into the database
    user.databaseData
      .save()
      .then((saved) => {
        //If it saved then respond with succeeded message
        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.Removed_item, indices)
        );
      })
      .catch((error) => {
        logger.warn(error);
        ResponseHandler.HandleResponse(
          user,
          res,
          new ResponseData(ResponseTypes.Server_Error)
        );
      });
  }
}

module.exports = ItemHandler;
