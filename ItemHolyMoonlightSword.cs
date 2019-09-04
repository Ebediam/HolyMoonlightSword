using BS;
using UnityEngine;

namespace HolyMoonlightSword
{

    public class ItemHolyMoonlightSword : MonoBehaviour
    {
       
        protected Item item;
        public ItemModuleHolyMoonlightSword module;

        public ItemData moonlightBeam;
        public Item moonlightBeamInstance;

        public Transform wrappings;
        public Transform moonlightBlade;

        public AudioSource changeSFX;
        public AudioSource spookySFX;

        public ParticleSystem particlesVFX;
        public ParticleSystem cloudyVFX;
        public ParticleSystem glowVFX;

        public Item leftHand;
        public Item rightHand;

        public bool lightBeam = false;
        public bool isMoonlight = false;
        public bool isChanging = false;



        public LineRenderer leftLine;
        private Vector3[] direction = new Vector3[2];



        public Transform parryPoint;

        public Vector3 startPoint = new Vector3();
        public Vector3 endPoint = new Vector3();
        protected void Awake()
        {
            startPoint = Vector3.zero;
            endPoint = Vector3.zero;
            item = this.GetComponent<Item>();            

            module = item.data.GetModule<ItemModuleHolyMoonlightSword>();

            parryPoint = item.transform.Find("ParryPoint");
            wrappings = item.transform.Find("Wrappings");
            moonlightBlade = item.transform.Find("MoonlightBlade");

            moonlightBlade.gameObject.SetActive(false);
            wrappings.gameObject.SetActive(true);

            moonlightBeam = Catalog.current.GetData<ItemData>("MoonlightBeam", true);

            changeSFX = item.transform.Find("ChangeSFX").GetComponent<AudioSource>();
            

            particlesVFX = item.transform.Find("ParticlesVFX").GetComponent<ParticleSystem>();
            cloudyVFX = item.transform.Find("CloudyVFX").GetComponent<ParticleSystem>();
            glowVFX = item.transform.Find("GlowVFX").GetComponent<ParticleSystem>();

            item.OnCollisionEvent += MoonlightCollision;
            item.OnHeldActionEvent += MoonlightAction;

            
            if (Creature.player && Player.local)
            {
                leftHand = Player.local.handLeft.itemHand.item;

                rightHand = Player.local.handRight.itemHand.item;
            }

            /*leftLine = Creature.player.gameObject.AddComponent<LineRenderer>();
            leftLine.receiveShadows = false;
            leftLine.material.color = Color.white;
            leftLine.startWidth = 0.005f;
            leftLine.endWidth = 0.005f;
            leftLine.startColor = Color.white;
            leftLine.endColor = Color.white;*/
        }


        void MoonlightAction(Interactor interactor, Handle handle, Interactable.Action action)
        {
            if (isMoonlight)
            {
                if(action == Interactable.Action.AlternateUseStart)
                {
                    lightBeam = true;
                }
            }
        }
       void MoonlightCollision(ref CollisionStruct collisionInstance)
       {
            if (collisionInstance.otherInteractiveObject && !isChanging)
            {
                if (collisionInstance.otherInteractiveObject == leftHand || collisionInstance.otherInteractiveObject == rightHand)
                {
                    if (collisionInstance.sourceColliderGroup.name == "Blade")
                    {
                        isChanging = true;
                        PlayFX();
                        Invoke("ChangeWeapon", 2f);

                    }

                    if (collisionInstance.sourceColliderGroup.name == "MoonlightBlade")
                    {
                        isChanging = true;
                        PlayFX();
                        Invoke("ChangeWeapon", 2f);


                    }
                }
            }

            
       }

        void PlayFX()
        {
            glowVFX.Play();
            cloudyVFX.Play();
            particlesVFX.Play();
            changeSFX.Play();
        }

        void StopFX()
        {
            glowVFX.Stop();
            cloudyVFX.Stop();
            particlesVFX.Stop();
            changeSFX.Stop();
        }


        void FixedUpdate()
        {
            if (isMoonlight)
            {
                if (lightBeam)
                {
                    if(startPoint == Vector3.zero && endPoint == Vector3.zero)
                    {
                        if(item.rb.velocity.magnitude > 8f)
                        {
                            startPoint = parryPoint.position;
                            Invoke("GetEndPoint", 0.2f);

                        }

                    }
                }
            }
        }

        public void GetEndPoint()
        {
            endPoint = parryPoint.position;
            lightBeam = false;
            moonlightBeamInstance = moonlightBeam.Instantiate(null);
            moonlightBeamInstance.gameObject.SetActive(true);
            moonlightBeamInstance.transform.position = (startPoint + endPoint) / 2f;
            moonlightBeamInstance.transform.LookAt(moonlightBeamInstance.transform.position + moonlightBeamInstance.transform.position - Creature.player.body.headBone.position);

            

            float zAngleBetween = Vector3.SignedAngle(moonlightBeamInstance.transform.right, endPoint - moonlightBeamInstance.transform.position, moonlightBeamInstance.transform.forward);

            moonlightBeamInstance.transform.Rotate(0f, 0f, zAngleBetween);


            moonlightBeamInstance.Throw(2f, Item.FlyDetection.Forced);
            moonlightBeamInstance.rb.AddForce(moonlightBeamInstance.transform.forward.normalized * 20f, ForceMode.VelocityChange);

            /*irection[0] = startPoint;
            direction[1] = endPoint;
            leftLine.SetPositions(direction);*/

            startPoint = Vector3.zero;
            endPoint = Vector3.zero;
        }

        public void AllowChange()
        {
            isChanging = false;
        }
        public void ChangeWeapon()
        {
            if (isMoonlight)
            {
                isMoonlight = false;
                moonlightBlade.gameObject.SetActive(false);
                wrappings.gameObject.SetActive(true);
                Invoke("AllowChange", 1f);

            }
            else
            {

                isMoonlight = true;
                moonlightBlade.gameObject.SetActive(true);
                wrappings.gameObject.SetActive(false);
                Invoke("AllowChange", 1f);
            }

            StopFX();
        }        
    }
}