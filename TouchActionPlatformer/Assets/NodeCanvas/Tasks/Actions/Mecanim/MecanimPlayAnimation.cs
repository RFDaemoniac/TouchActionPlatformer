using UnityEngine;
 
namespace NodeCanvas.Actions{
 
    [Name("Play Animation")]
    public class MecanimPlayAnimation : MecanimActions{
     
        public int layerIndex;
        [RequiredField]
        public string stateName;
        [SliderField(0,1)]
        public float transitTime = 0.25f;
        public bool waitUntilFinish;
     
        private AnimatorStateInfo stateInfo;
        private bool played;
     
        protected override string info{
            get {return "Anim '" + stateName + "'";}
        }
     
        protected override void OnExecute(){
            played = false;
            var current = animator.GetCurrentAnimatorStateInfo(layerIndex);
            animator.CrossFade(stateName, transitTime/current.length, layerIndex);
        }
     
        protected override void OnUpdate(){
         
            stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
         
            if (waitUntilFinish){
             
                if (stateInfo.IsName(stateName)){
            
					played = true;
                    if(elapsedTime >= (stateInfo.length / animator.speed))
                        EndAction();              

                } else if (played) {

                    EndAction();
                }
             
            } else {

                if (elapsedTime >= transitTime)
                    EndAction();
            }
        }
    }
}