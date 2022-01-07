using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniGLTF;
using UnityEngine;


namespace ValheimVRM
{
    public sealed class VRMHandler : MonoBehaviour
    {

		public GameObject vrmGameObject { get; private set;}
		public string Path { get; set; }
		public float Scale { get; set; }

		public VRMHandler()
        {


		}
		public void loadVRM()
        {
			try
			{
				// I converted GltfParser to the newer way to import. It handles in 3 steps,
				// Details on how to import are located: https://vrm-c.github.io/UniVRM/en/gltf/0_82_glb_import.html
				Debug.Log("Parsing GLTF...");
				var data = new AutoGltfFileParser(Path).Parse();
				Debug.Log("Parse Check");

				// 2. GltfParser のインスタンスを引数にして VRMImporterContext を作成します。
				//    VRMImporterContext は VRM のロードを実際に行うクラスです。
				using (var context = new UniGLTF.ImporterContext(data))
				{
					// 3. Load 関数を呼び出し、VRM の GameObject を生成します。
					var instance = context.Load();

					// 4. （任意） SkinnedMeshRenderer の UpdateWhenOffscreen を有効にできる便利関数です。
					instance.EnableUpdateWhenOffscreen();

					// 5. VRM モデルを表示します。
					instance.ShowMeshes();

					// The next section to destroy is probably outdated. I believe it was replaced with 'GameObject.Destroy(instance);'
					// 6. VRM の GameObject が実際に使用している UnityEngine.Object リソースの寿命を VRM の GameObject に紐付けます。
					//    つまり VRM の GameObject の破棄時に、実際に使用しているリソース (Texture, Material, Mesh, etc) をまとめて破棄することができます。
					// instance.DisposeOnGameObjectDestroyed();

					// GameObject.Destroy(instance);

					instance.Root.transform.localScale *= Scale;

					Debug.Log("[ValheimVRM] VRM読み込み成功");
					Debug.Log("[ValheimVRM] VRMファイルパス: " + Path);

					// 7. Root の GameObject を return します。
					//    Root の GameObject とは VRMMeta コンポーネントが付与されている GameObject のことです。
					vrmGameObject = instance.Root;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex);
			}
		}

		private void onDestroy()
        {

        }
    }
}
