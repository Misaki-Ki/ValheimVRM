using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ValheimVRMod;
using RootMotion.FinalIK;
using ValheimVRMod.Scripts;

namespace ValheimVRM
{

    [DefaultExecutionOrder(int.MaxValue)]
    public class VRMAnimationSync : MonoBehaviour
    {
        private Animator orgAnim, vrmAnim;
        private HumanPoseHandler orgPose, vrmPose;
        private HumanPose hp = new HumanPose();
        private bool ragdoll;
        private float offset;
        
        public String playerName { get; set; }
        [System.ComponentModel.DefaultValue(0.0)]
        public float vrHipOffset { get; set; }

        private bool _isFirstPerson = ValheimVRMod.VRCore.VRPlayer.inFirstPerson;
        private Vector3 startingHeadScale = Vector3.one;
        private Player playerInstance;


        public void Setup(Animator orgAnim, Player playerInstance, bool isRagdoll = false, float offset = 0.0f)
        {


            Debug.Log("Setup Start");
            this.ragdoll = isRagdoll; Debug.Log("Ragdoll " + isRagdoll);
            this.offset = offset; Debug.Log("Offset");
            this.orgAnim = orgAnim; Debug.Log("orgAnim"); Debug.Log(orgAnim);
            this.vrmAnim = GetComponent<Animator>(); Debug.Log("GetAnimator");
            this.playerInstance = playerInstance;
            Debug.Log(vrmAnim);

            Component[] components = this.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                Debug.Log(component.ToString());
            }

            Component[] components2 = this.GetComponentsInChildren(typeof(Component));
            foreach (Component component in components2)
            {
                Debug.Log(component.ToString());
            }


            this.vrmAnim.applyRootMotion = true; Debug.Log("ApplyRootMotion");
            this.vrmAnim.updateMode = orgAnim.updateMode;
            this.vrmAnim.feetPivotActive = orgAnim.feetPivotActive;
            this.vrmAnim.layersAffectMassCenter = orgAnim.layersAffectMassCenter;
            this.vrmAnim.stabilizeFeet = orgAnim.stabilizeFeet;
            startingHeadScale = vrmAnim.GetBoneTransform(HumanBodyBones.Head).localScale;
            Debug.Log("Setup Complete");

            PoseHandlerCreate(orgAnim, vrmAnim);


        }

        void PoseHandlerCreate(Animator org, Animator vrm)
        {
            OnDestroy();
            orgPose = new HumanPoseHandler(org.avatar, org.transform);
            vrmPose = new HumanPoseHandler(vrm.avatar, vrm.transform);

            Debug.Log("Original: " + org.gameObject.name + " , Chest: " + org.GetBoneTransform(HumanBodyBones.Chest).name);
            Debug.Log("VRM: " + vrm.gameObject.name + " , Chest: " + vrm.GetBoneTransform(HumanBodyBones.Chest).name);
        }

        void OnDestroy()
        {
            if (orgPose != null)
                orgPose.Dispose();
            if (vrmPose != null)
                vrmPose.Dispose();

        }



        private float CalcFootSub()
        {
            var orgRightFoot = orgAnim.GetBoneTransform(HumanBodyBones.RightFoot).position;
            var orgLeftFoot = orgAnim.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            var orgArgFoot = (orgRightFoot + orgLeftFoot) * 0.5f;

            var vrmRightFoot = vrmAnim.GetBoneTransform(HumanBodyBones.RightFoot).position;
            var vrmLeftFoot = vrmAnim.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            var vrmArgFoot = (vrmRightFoot + vrmLeftFoot) * 0.5f;

            return (orgArgFoot - vrmArgFoot).y;
        }


        // Attempting to tell if it's in VR by comparing the position of the camera to the head bone.
        /* private bool isVR(GameObject camera)
        {
			

			if (camera == null || camera.gameObject == null)
            {
				Debug.Log("VR Camera not found.");
				return false;
            }

			var orgHead = orgAnim.GetBoneTransform(HumanBodyBones.Head).position;
			



			Debug.Log("Head Position: " + orgHead + "||  VRCamera Position:    " + VR_Camera.transform.position);

			return true;
        } */





        /*		IEnumerator Start()
                {




                }*/
        private static List<int> alreadyHashes = new List<int>();

        // 最初: -161139084
        // 通常: 229373857
        // 最初立ち上がり: -1536343465
        // 立ち上がり: -805461806
        // 座り始め: 890925016
        // 座り: -1544306596
        // 椅子: -1829310159
        // ベッド寝始め: 337039637
        // ベッド: -1603096
        // ベッド起き上がり: -496559199
        // Crouch: -2015693266

        private static List<int> adjustHipHashes = new List<int>()
        {
            -1536343465,
            890925016,
            -1544306596,
            -1829310159,
            337039637,
            -1603096,
            -496559199,
        };

        void Update()
        {

            _isFirstPerson = ValheimVRMod.VRCore.VRPlayer.inFirstPerson;
            // This block in particular is what prevented VRM from working in VR. By making an if statement that checks for firstperson
            // we can avoid having the original model being moved.
            Player localPlayer = Player.m_localPlayer;
            


            if ((!ragdoll && !localPlayer) || (!ragdoll && !_isFirstPerson))
            {
                var vrIker = GetComponent<VRIK>();

                if (vrIker != null)
                {
                    if (vrIker.isActiveAndEnabled)
                    {
                        return;
                    }

                    orgTransPosFromVRMTransPos();
                }


                
                
            }





        }

        private void orgTransPosFromVRMTransPos()
        {
            for (var i = 0; i < 55; i++)
            {
                var orgTrans = orgAnim.GetBoneTransform((HumanBodyBones)i);
                var vrmTrans = vrmAnim.GetBoneTransform((HumanBodyBones)i);

                if (i > 0 && orgTrans != null && vrmTrans != null)
                {
                    orgTrans.position = vrmTrans.position;
                }
            }

        }

        private bool isLocalPlayer()
        {
            if (Player.m_localPlayer == playerInstance)
            {
                return true;
            }

            return false;
        }

        void LateUpdate()
        {
            string playerName = null;

            Player localPlayer = Player.m_localPlayer;

            vrmAnim.transform.localPosition = Vector3.zero;
            var orgHipPos = orgAnim.GetBoneTransform(HumanBodyBones.Hips).position;


            orgPose.GetHumanPose(ref hp);
            vrmPose.SetHumanPose(ref hp);





            var nameHash = orgAnim.GetCurrentAnimatorStateInfo(0).shortNameHash;
            var adjustFromHip = adjustHipHashes.Contains(nameHash);

            //if (!alreadyHashes.Contains(nameHash))
            //{
            //	alreadyHashes.Add(nameHash);
            //	Debug.Log(orgAnim.GetCurrentAnimatorClipInfo(0)[0].clip.name + ": " + nameHash);
            //}

            var vrmHip = vrmAnim.GetBoneTransform(HumanBodyBones.Hips);
            if (adjustFromHip)
            {
                vrmHip.position = orgHipPos;
            }

            var adjustHeight = 0.0f;
            if (nameHash == 890925016 || nameHash == -1544306596 || nameHash == -1829310159) // Sitting
            {
                adjustHeight += 0.1f;
            }

            if (!adjustFromHip)
            {
                adjustHeight = CalcFootSub();
            }

            var pos = vrmHip.position;
            pos.y += adjustHeight;
            vrmHip.position = pos;

            if ((!ragdoll && !isLocalPlayer()) || (!ragdoll && !_isFirstPerson)) 
            {
                for (var i = 0; i < 55; i++)
                {
                    var orgTrans = orgAnim.GetBoneTransform((HumanBodyBones)i);
                    var vrmTrans = vrmAnim.GetBoneTransform((HumanBodyBones)i);

                    if (i > 0 && orgTrans != null && vrmTrans != null)
                    {
                        orgTrans.position = vrmTrans.position;
                    }
                }
            }




            vrmAnim.transform.localPosition += Vector3.up * offset;


            // Here we'll do some corrections to VR. Shrinking the head to get it out of view. I need to make a clone for the shadows still.
            // I'm going to include an Y offset, it should help make various models feel a bit better.


            if (isLocalPlayer() && _isFirstPerson)
            {
                // Debug.Log("Local Player and First Person.");
                if (playerName != null)
                {
                 //   Debug.Log("Player Name: " + playerName);
                    this.vrmAnim.GetBoneTransform(HumanBodyBones.Hips).localPosition += Vector3.up * vrHipOffset;
                }
                
                
                this.vrmAnim.GetBoneTransform(HumanBodyBones.Head).localScale = new Vector3(0.001f, 0.001f, 0.0001f);

               // Debug.Log(vrmAnim.GetBoneTransform(HumanBodyBones.Head).name + " set to " + vrmAnim.GetBoneTransform(HumanBodyBones.Head).localScale);
                

            }

            else
            {
               //  Debug.Log("else branch");
                vrmAnim.GetBoneTransform(HumanBodyBones.Head).localScale = startingHeadScale;
              //  Debug.Log("StartingHeadScale = " + startingHeadScale);
            }

            /*			if (localPlayer && _isFirstPerson)
                        {
                            if (gameObject.GetComponent<VRIK>() == null)
                            {

                                vrik = ValheimVRMod.Scripts.VrikCreator.initialize(this.gameObject, leftHand, rightHand, head);
                                Debug.Log("VRIK Initialized to VRM.");




                                *//*

                                vrik.enabled = true;



                                var vrikReferences = vrik.references.GetTransforms();

                                for (var i = 0; i < vrikReferences.Length; i++)
                                {
                                    Debug.Log(vrikReferences[i]);
                                }

                                 *//*

                            }

                            else
                            {
                                if (vrik.enabled == false)
                                {


                                    vrik.enabled = true;
                                }

                                rightHand.position = ValheimVRMod.VRCore.VRPlayer.RightHandGameObject.transform.position;
                                leftHand.position = ValheimVRMod.VRCore.VRPlayer.LeftHandGameObject.transform.position;
                                head.position = ValheimVRMod.VRCore.VRPlayer.VrCameraRig.transform.position;
                            }


                        }
                        Debug.Log("===");
                        Debug.Log("VRM Hip Bone position: " + vrmAnim.GetBoneTransform((HumanBodyBones.Hips)).position);
                        Debug.Log("VRM Hip Bone Local position: " + vrmAnim.GetBoneTransform((HumanBodyBones.Hips)).localPosition);
                        Debug.Log(gameObject.name + "position : " + gameObject.transform.position);*/

        }





    }
}
