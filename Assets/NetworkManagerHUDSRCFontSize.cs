﻿using System;
using System.ComponentModel;

namespace UnityEngine.Networking {
	[AddComponentMenu("Network/NetworkManagerHUDSRCFontSize")]
	[RequireComponent(typeof(NetworkManager))]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class NetworkManagerHUDSRCFontSize : MonoBehaviour
	{
		public NetworkManager manager;
		[SerializeField]
		public bool showGUI = true;
		[SerializeField]
		public int offsetX;
		[SerializeField]
		public int offsetY;

		// Runtime variable
		bool m_ShowServer;

		public float wr = 2f, hr = 2f , sr = 4f, spacing = 24f;

		void Awake()
		{
			manager = GetComponent<MyNetworkManager> ();
		}

		public void OnGUI(){

			if (!showGUI) {
				return;
			}

			GUIStyle buttonStyle = GUI.skin.button;
			buttonStyle.normal.textColor = Color.white;
			buttonStyle.fontSize = 30;

			GUI.skin.textField.fontSize = 30;

//			float wr = 2f;
//			float hr = 2f;
//			const float sr = 4.0f;

			float xpos = 5 + offsetX;
			float ypos = 20 + offsetY;
//			const float spacing = 24.0f * sr;

			bool noConnection = (manager.client == null || manager.client.connection == null ||
				manager.client.connection.connectionId == -1);

			if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
			{
				if (noConnection)
				{
					if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
					{
						if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "LAN Host(H)"))
						{
							manager.StartHost();
						}
						ypos += spacing;
					}

					if (GUI.Button(new Rect(xpos, ypos, 105 * wr, 20 * hr), "LAN Client(C)"))
					{
						manager.StartClient();
					}

					manager.networkAddress = GUI.TextField(new Rect(xpos + 105 * wr + spacing, ypos, 95 * wr, 20 * hr), manager.networkAddress);
					ypos += spacing;

					if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
					{
						// cant be a server in webgl build
						GUI.Box(new Rect(xpos, ypos, 200 * wr, 25 * hr), "(  WebGL cannot be server  )");
						ypos += spacing;
					}
					else
					{
						if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "LAN Server Only(S)"))
						{
							manager.StartServer();
						}
						ypos += spacing;
					}
				}
				else
				{
					GUI.Label(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..");
					ypos += spacing;


					if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Cancel Connection Attempt"))
					{
						manager.StopClient();
					}
				}
			}
			else
			{
				if (NetworkServer.active)
				{
					string serverMsg = "Server: port=" + manager.networkPort;
					if (manager.useWebSockets)
					{
						serverMsg += " (Using WebSockets)";
					}
					GUI.Label(new Rect(xpos, ypos, 300 * wr, 20 * hr), serverMsg);
					ypos += spacing;
				}
				if (manager.IsClientConnected())
				{
					GUI.Label(new Rect(xpos, ypos, 300 * wr, 20 * hr), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
					ypos += spacing;
				}
			}

			if (manager.IsClientConnected() && !ClientScene.ready)
			{
				if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Client Ready"))
				{
					ClientScene.Ready(manager.client.connection);

					if (ClientScene.localPlayers.Count == 0)
					{
						ClientScene.AddPlayer(0);
					}
				}
				ypos += spacing;
			}

			if (NetworkServer.active || manager.IsClientConnected())
			{
				if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Stop (X)"))
				{
					manager.StopHost();
				}
				ypos += spacing;
			}

			if (!NetworkServer.active && !manager.IsClientConnected() && noConnection)
			{
				ypos += 10;

				if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
				{
					GUI.Box(new Rect(xpos - 5, ypos, 220 * wr, 25 * hr), "(WebGL cannot use Match Maker)");
					return;
				}

				if (manager.matchMaker == null)
				{
					if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Enable Match Maker (M)"))
					{
						manager.StartMatchMaker();
					}
					ypos += spacing;
				}
				else
				{
					if (manager.matchInfo == null)
					{
						if (manager.matches == null)
						{
							if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Create Internet Match"))
							{
								manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 0, manager.OnMatchCreate);
							}
							ypos += spacing;

							GUI.Label(new Rect(xpos, ypos, 100 * wr, 20 * hr), "Room Name:");
							manager.matchName = GUI.TextField(new Rect(xpos + 100 * wr + spacing, ypos, 100 * wr, 20 * hr), manager.matchName);
							ypos += spacing;

							ypos += 10;

							if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Find Internet Match"))
							{
								manager.matchMaker.ListMatches(0, 20, "", false, 0, 0, manager.OnMatchList);
							}
							ypos += spacing;
						}
						else
						{
							for (int i = 0; i < manager.matches.Count; i++)
							{
								var match = manager.matches[i];
								if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Join Match:" + match.name))
								{
									manager.matchName = match.name;
									manager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, manager.OnMatchJoined);
								}
								ypos += spacing;
							}

							if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Back to Match Menu"))
							{
								manager.matches = null;
							}
							ypos += spacing;
						}
					}

					if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Change MM server"))
					{
						m_ShowServer = !m_ShowServer;
					}
					if (m_ShowServer)
					{
						ypos += spacing;
						if (GUI.Button(new Rect(xpos, ypos, 100 * wr, 20 * hr), "Local"))
						{
							manager.SetMatchHost("localhost", 1337, false);
							m_ShowServer = false;
						}
						ypos += spacing;
						if (GUI.Button(new Rect(xpos, ypos, 100 * wr, 20 * hr), "Internet"))
						{
							manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
							m_ShowServer = false;
						}
						ypos += spacing;
						if (GUI.Button(new Rect(xpos, ypos, 100 * wr, 20 * hr), "Staging"))
						{
							manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
							m_ShowServer = false;
						}
					}

					ypos += spacing;

					GUI.Label(new Rect(xpos, ypos, 300 * wr, 20 * hr), "MM Uri: " + manager.matchMaker.baseUri);
					ypos += spacing;

					if (GUI.Button(new Rect(xpos, ypos, 200 * wr, 20 * hr), "Disable Match Maker"))
					{
						manager.StopMatchMaker();
					}
					ypos += spacing;
				}
			}
		}	//Awake()
	}
}