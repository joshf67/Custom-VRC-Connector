# VRCUrl impact testing

[Return to negatives](Introduction.md#negatives)

### Tested on:
- CPU: 5800x3d
- GPU: RTX 4090

Starting URL (38 characters) = "http//\*\*\*.\*\*\*.\*.\*\*\*:\*\*\*\*\*/sendMessage="

### 21 bit message lengths = 2097152 VRCUrls:
- File size: 2.65MB
- Build time: 2 minutes 10 seconds

### 20 bit message lengths = 1048576 VRCUrls:
- File size: 1.44MB
- Build time: 1 minutes 48 seconds

### None:
- File size: 212.87kb
- Build time: 41 seconds

### Assuming linear scaling (which is probably innacurate) 1 VRCUrl:
- File size: 1.15 bytes
- Build time: 0.02ms
