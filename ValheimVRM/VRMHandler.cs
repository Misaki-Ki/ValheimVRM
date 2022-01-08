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

			vrmGameObject = go;

		}
    }
}
