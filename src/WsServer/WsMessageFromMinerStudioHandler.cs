﻿using NTMiner.Core;
using NTMiner.Core.Daemon;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Ws;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class WsMessageFromMinerStudioHandler {
        static WsMessageFromMinerStudioHandler() {
        }

        private static readonly Dictionary<string, Action<IMinerStudioSession, Guid, WsMessage>>
            _handlers = new Dictionary<string, Action<IMinerStudioSession, Guid, WsMessage>>(StringComparer.OrdinalIgnoreCase) {
                [WsMessage.GetConsoleOutLines] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetConsoleOutLines)}");
                        MqBufferRoot.GetConsoleOutLines(new AfterTimeRequest {
                            AfterTime = afterTime,
                            ClientId = wrapperClientIdData.ClientId,
                            LoginName = session.LoginName
                        });
                    }
                },
                [WsMessage.FastGetConsoleOutLines] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.FastGetConsoleOutLines)}");
                        AppRoot.OperationMqSender.SendFastGetConsoleOutLines(session.LoginName, wrapperClientIdData.ClientId, studioId, afterTime);
                    }
                },
                [WsMessage.GetLocalMessages] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetLocalMessages)}");
                        MqBufferRoot.GetLocalMessages(new AfterTimeRequest {
                            AfterTime = afterTime,
                            ClientId = wrapperClientIdData.ClientId,
                            LoginName = session.LoginName
                        });
                    }
                },
                [WsMessage.FastGetLocalMessages] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.FastGetLocalMessages)}");
                        AppRoot.OperationMqSender.SendFastGetLocalMessages(session.LoginName, wrapperClientIdData.ClientId, studioId, afterTime);
                    }
                },
                [WsMessage.GetOperationResults] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetOperationResults)}");
                        MqBufferRoot.GetOperationResults(new AfterTimeRequest {
                            AfterTime = afterTime,
                            ClientId = wrapperClientIdData.ClientId,
                            LoginName = session.LoginName
                        });
                    }
                },
                [WsMessage.FastGetOperationResults] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out long afterTime)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.FastGetOperationResults)}");
                        AppRoot.OperationMqSender.SendFastGetOperationResults(session.LoginName, wrapperClientIdData.ClientId, studioId, afterTime);
                    }
                },
                [WsMessage.GetDrives] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetDrives)}");
                        AppRoot.OperationMqSender.SendGetDrives(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.GetLocalIps] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetLocalIps)}");
                        AppRoot.OperationMqSender.SendGetLocalIps(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.GetSpeed] = (session, studioId, message) => {
                    if (message.TryGetData(out List<Guid> clientIds)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetSpeed)}");
                        MqBufferRoot.UserGetSpeed(new UserGetSpeedRequest {
                            StudioId = studioId,
                            LoginName = session.LoginName,
                            ClientIds = clientIds
                        });
                    }
                },
                [WsMessage.EnableRemoteDesktop] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.EnableRemoteDesktop)}");
                        AppRoot.OperationMqSender.SendEnableRemoteDesktop(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.BlockWAU] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.BlockWAU)}");
                        AppRoot.OperationMqSender.SendBlockWAU(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.SetVirtualMemory] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out Dictionary<string, int> data)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.SetVirtualMemory)}");
                        AppRoot.OperationMqSender.SendSetVirtualMemory(session.LoginName, wrapperClientIdData.ClientId, studioId, data);
                    }
                },
                [WsMessage.SetLocalIps] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out List<LocalIpInput> data)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.SetLocalIps)}");
                        AppRoot.OperationMqSender.SendSetLocalIps(session.LoginName, wrapperClientIdData.ClientId, studioId, data);
                    }
                },
                [WsMessage.SwitchRadeonGpu] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientIdData) && wrapperClientIdData.TryGetData(out bool on)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.SwitchRadeonGpu)}");
                        AppRoot.OperationMqSender.SendSwitchRadeonGpu(session.LoginName, wrapperClientIdData.ClientId, studioId, on);
                    }
                },
                [WsMessage.GetSelfWorkLocalJson] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetSelfWorkLocalJson)}");
                        AppRoot.OperationMqSender.SendGetSelfWorkLocalJson(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.SaveSelfWorkLocalJson] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out WorkRequest workRequest)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.SaveSelfWorkLocalJson)}");
                        AppRoot.OperationMqSender.SendSaveSelfWorkLocalJson(session.LoginName, wrapperClientData.ClientId, studioId, workRequest);
                    }
                },
                [WsMessage.GetGpuProfilesJson] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.GetGpuProfilesJson)}");
                        AppRoot.OperationMqSender.SendGetGpuProfilesJson(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.SaveGpuProfilesJson] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out string json)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.SaveGpuProfilesJson)}");
                        AppRoot.OperationMqSender.SendSaveGpuProfilesJson(session.LoginName, wrapperClientData.ClientId, studioId, json);
                    }
                },
                [WsMessage.SetAutoBootStart] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out SetAutoBootStartRequest body)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.SetAutoBootStart)}");
                        AppRoot.OperationMqSender.SendSetAutoBootStart(session.LoginName, wrapperClientData.ClientId, studioId, body);
                    }
                },
                [WsMessage.RestartWindows] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.RestartWindows)}");
                        AppRoot.OperationMqSender.SendRestartWindows(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.ShutdownWindows] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.ShutdownWindows)}");
                        AppRoot.OperationMqSender.SendShutdownWindows(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.UpgradeNTMiner] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out string ntminerFileName)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.UpgradeNTMiner)}");
                        AppRoot.OperationMqSender.SendUpgradeNTMiner(session.LoginName, wrapperClientData.ClientId, studioId, ntminerFileName);
                    }
                },
                [WsMessage.StartMine] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientIdData wrapperClientData) && wrapperClientData.TryGetData(out Guid workId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.StartMine)}");
                        AppRoot.OperationMqSender.SendStartMine(session.LoginName, wrapperClientData.ClientId, studioId, workId);
                    }
                },
                [WsMessage.StopMine] = (session, studioId, message) => {
                    if (message.TryGetData(out WrapperClientId wrapperClientId)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.StopMine)}");
                        AppRoot.OperationMqSender.SendStopMine(session.LoginName, wrapperClientId.ClientId, studioId);
                    }
                },
                [WsMessage.QueryClientDatas] = (session, studioId, message) => {
                    if (message.TryGetData(out QueryClientsRequest query)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.QueryClientDatas)}");
                        AppRoot.MinerClientMqSender.SendQueryClientsForWs(QueryClientsForWsRequest.Create(query, session.LoginName, studioId, session.WsSessionId));
                    }
                },
                [WsMessage.AutoQueryClientDatas] = (session, studioId, message) => {
                    if (message.TryGetData(out QueryClientsRequest query)) {
                        ServerRoot.IfStudioClientTestIdLogElseNothing(studioId, $"{nameof(WsMessage)}.{nameof(WsMessage.AutoQueryClientDatas)}");
                        MqBufferRoot.AutoQueryClientDatas(QueryClientsForWsRequest.Create(query, session.LoginName, studioId, session.WsSessionId));
                    }
                }
            };

        public static bool TryGetHandler(string wsMessageType, out Action<IMinerStudioSession, Guid, WsMessage> handler) {
            return _handlers.TryGetValue(wsMessageType, out handler);
        }
    }
}
