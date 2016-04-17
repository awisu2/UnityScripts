/// <summary>
/// AnimatorUtil.cs
/// AnimatorUtilクラス
/// </summary>
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor;
using System.Collections.Generic;
using org.a2dev.UnityScripts.Util;

namespace org.a2dev.UnityScripts.Editor
{
    public class AnimatorUtil
    {
        // 座標
        Dictionary<AnimatorStateMachine, StatePosition> statePositions;

		/// <summary>
		/// StatePosition
		/// stateやstateMachineの座標を扱う
		/// </summary>
        public class StatePosition
        {
            // stateのサイズ
            public static Vector2 stateSize = new Vector2(200f, 40f);

            // state同士の間隔
            public static Vector2 margin = new Vector2(10f, 15f);

			// stateを並べる際のズレ
            public static float diff = 5f;

            // state同士の間隔
            public static Vector2 tipSize = stateSize + margin;

            public int x { get; set; }
            public int y { get; set; }

			// コンストラクタ
            public StatePosition()
            {
                x = 0;
                y = 0;
            }

			// Vector2での座標
            public Vector2 position
            {
                get
                {
                    // transition(遷移線)がかぶるのでズレを作る
                    float deviation = diff * Mathf.Pow((float)y, 2f);
                    if (y < 0)
                    {
                        deviation *= -1;
                    }
                    return new Vector2(tipSize.x * x + deviation, tipSize.y * y);
                }
            }

            public void SetIndex(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public void IncrementX()
            {
                ++x;
            }

            public void IncrementY()
            {
                ++y;
            }
        }

		// コンストラクタ
        public AnimatorUtil()
        {
            statePositions = new Dictionary<AnimatorStateMachine, StatePosition>();
        }

        // コントローラを作成
        public AnimatorController CreateAnimatorController(string path)
        {
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            for (int i = 0; i < controller.layers.Length; ++i)
            {
                InitStatesPosition(controller.layers[i].stateMachine, false);
            }
            return controller;
        }

        // AnimationClipの作成(.anim)
        public AnimationClip CreateAnimationClip(string path)
        {
             AnimationClip clip = new AnimationClip();
             FileUtilll.GetExtension(path);
             AssetDatabase.CreateAsset(clip, path);
             return clip;
        }
        

        // stateMachineを追加(ディレクトリみたいなもの)
        public AnimatorStateMachine AddStateMachine(AnimatorStateMachine machine, string machineName)
        {
            StatePosition statePosition = GetStatePosition(machine);
            var newMachine = machine.AddStateMachine(machineName, statePosition.position);
            statePosition.IncrementY();

            InitStatesPosition(newMachine);

            return newMachine;
        }

        // stateを追加(motion( AnimationClip or BlendTree)を格納するアニメーション本体)
        public AnimatorState AddState(AnimatorStateMachine machine, string stateName)
        {
            StatePosition statePosition = GetStatePosition(machine);
            var state = machine.AddState(stateName, statePosition.position);
            statePosition.IncrementY();

            return state;
        }

		// transitionを追加
        public AnimatorTransition AddStateMachineTransition(AnimatorStateMachine machine,
			AnimatorStateMachine sourceMachine, AnimatorStateMachine destinationMachine)
        {
            var transition = machine.AddStateMachineTransition(sourceMachine, destinationMachine);
            return transition;
        }

        // StateMachine内の基本的に存在するStateの座標を初期化する
        public void InitStatesPosition(AnimatorStateMachine machine, bool isChildMachine = true)
        {
            StatePosition statePosition = GetStatePosition(machine);
            machine.entryPosition = statePosition.position;

            statePosition.IncrementY();
            machine.anyStatePosition = statePosition.position;

            statePosition.IncrementY();
            machine.exitPosition = statePosition.position;

            if (isChildMachine)
            {
                statePosition.SetIndex(1, -1);
                machine.parentStateMachinePosition = statePosition.position; ;
            }

            statePosition.SetIndex(1, 0);
        }

		// statePositioinに座標Indexをセット
        public void SetStatePosition(AnimatorStateMachine machine, int x, int y)
        {
            StatePosition statePosition = GetStatePosition(machine);
            statePosition.SetIndex(x, y);
        }

		// statePositionを取得
        public StatePosition GetStatePosition(AnimatorStateMachine machine)
        {
            if (statePositions.ContainsKey(machine) == false)
            {
                statePositions.Add(machine, new StatePosition());
            }

            return statePositions[machine];
        }
		
		public enum stateTransitionType
		{
			ANY,
			ENTRY,
			EXIT,
		}
		
		// StateTransitionの追加
		public AnimatorStateTransition AddStateTransitionSimple(AnimatorStateMachine machine, AnimatorState state, string paramName, int duration = 0)
		{
			var transition = machine.AddAnyStateTransition(state);
			transition.AddCondition(AnimatorConditionMode.If, 0, paramName);
			transition.duration = duration;
			
			return transition;
		}
        
        // イベントの作成
        public AnimationEvent CreateAnimationEvent(string name, float time = 0f)
        {
            var evt = new AnimationEvent();
            evt.functionName = name;
            evt.time = time;
            return evt;
        }
        
        // イベントの作成
        public AnimationEvent CreateAnimationEvent(string name, int param, float time)
        {
            var evt = CreateAnimationEvent(name, time);
            evt.intParameter = param;
            return evt;
        }
        
        // イベントの作成
        public AnimationEvent CreateAnimationEvent(string name, string param, float time)
        {
            var evt = CreateAnimationEvent(name, time);
            evt.stringParameter = param;
            return evt;
        }

        // イベントの作成
        public AnimationEvent CreateAnimationEvent(string name, float param, float time)
        {
            var evt = CreateAnimationEvent(name, time);
            evt.floatParameter = param;
            return evt;
        }

        // イベントの作成
        public AnimationEvent CreateAnimationEvent(string name, Object param, float time)
        {
            var evt = CreateAnimationEvent(name, time);
            evt.objectReferenceParameter = param;
            return evt;
        }


    }
}
