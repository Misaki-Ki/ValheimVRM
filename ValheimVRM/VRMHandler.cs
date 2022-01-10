using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniGLTF;
using VRM;
using UnityEngine;


namespace ValheimVRM
{
    public sealed class VRMHandler : MonoBehaviour
    {

		public GameObject vrmGameObject { get; private set;}
		private String path;
		private float scale;
		private bool isVrm;

		public VRMHandler(String path, float scale, bool isVrm)
        {
			this.path = path;
			this.scale = scale;
			this.isVrm = isVrm;
			loadVRM();

		}

        public static void loadExisting(byte[] bytes, float scale)
        {
            // TODO: Load from existing.  
        }

		async public void loadVRM()
        {
			GltfData data;

			try

			{
				data = new AutoGltfFileParser(path).Parse();
			}

			catch (Exception ex)

			{
				Debug.LogWarningFormat("parse error: {0}", ex);
				return;
			}

			if (isVrm) {
				try
				{

					var vrm = new VRMData(data);
					using (var context = new VRMImporterContext(vrm))
					{
						var meta = await context.ReadMetaAsync();
						Debug.LogFormat("meta: title:{0}", meta.Title);
						var loaded = await context.LoadAsync();
						loaded.EnableUpdateWhenOffscreen();

						loaded.ShowMeshes();

						// GameObject.Destroy(instance);

						loaded.Root.transform.localScale *= scale;



						OnLoad(loaded.gameObject);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			}
		}

		private void OnLoad(GameObject go)
        {
			if (vrmGameObject != null)
			{
				GameObject.Destroy(vrmGameObject.gameObject);
			}
            var leftHand = GameObject.Find("L_Hand");
            var RightHand = GameObject.Find("R_Hand");

            /*
            VrmToValHand(leftHand);
            VrmToValHand(RightHand);
            */
            

            vrmGameObject = go;

            

        }

       /* public static void VrmToValHand(GameObject gameObject)
        {
            var boneNameConversionDict = new Dictionary<string, string>(){
         {"L_Hand", "LeftHand"},
            {"L_IndexProximal", "LeftHandIndex1"},
            {"L_IndexIntermediate", "LeftHandIndex2" },
            {"L_IndexDistal", "LeftHandIndex3"},

            {"L_LittleProximal", "LeftHandPinky1" },
            {"L_LittleIntermediate", "LeftHandPinky2" },
            {"L_LittleDistal", "LeftHandPinky3"},

            {"L_MiddleProximal", "LeftHandMiddle1"},
            {"L_MiddleIntermediate","LeftHandMiddle2"},
            {"L_MiddleDistal","LeftHandMiddle3"},

            {"L_RingProximal", "LeftHandRing1" },
            {"L_RingIntermediate", "LeftHandRing2"},
            {"L_RingDistal", "LeftHandRing3"},

            {"L_ThumbProximal1", "LeftHandThumb1" },
            {"L_ThumbIntermediate1", "LeftHandThumb2"},
            {"L_ThumbDistal1", "LeftHandThumb3"},


            {"R_Hand", "RightHand"},
            {"R_IndexProximal", "RightHandIndex1"},
            {"R_IndexIntermediate", "RightHandIndex2" },
            {"R_IndexDistal", "RightHandIndex3"},

            {"R_LittleProximal", "RightHandPinky1" },
            {"R_LittleIntermediate", "RightHandPinky2" },
            {"R_LittleDistal", "RightHandPinky3"},

            {"R_MiddleProximal", "RightHandMiddle1"},
            {"R_MiddleIntermediate","RightHandMiddle2"},
            {"R_MiddleDistal","RightHandMiddle3"},

            {"R_RingProximal", "RightHandRing1" },
            {"R_RingIntermediate", "RightHandRing2"},
            {"R_RingDistal", "RightHandRing3"},

            {"R_ThumbProximal1", "RightHandThumb1" },
            {"R_ThumbIntermediate1", "RightHandThumb2"},
            {"R_ThumbDistal1", "RightHandThumb3"}
        };

            // gameObject.name = boneNameConversionDict[gameObject.name];
            


        }
    }*/
}
