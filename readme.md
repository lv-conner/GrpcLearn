# Grpc Learn
## 发现问题：
- 1.在请求消息和相应消息都是Stream的情况下，以release运行，会出现Response Read问题。此问题仅在Grpc.Core.Server Host下发生