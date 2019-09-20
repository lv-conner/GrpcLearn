# Grpc Learn
## 发现问题：
- 1.在请求消息和相应消息都是Stream的情况下，以release运行，会出现Response Read问题。此问题仅在Grpc.Core.Server Host下发生

- 问题已解决。在升级到.net core 3.0 rc1后一切工作正常(目前尚无证据证明与此次升级有关)。