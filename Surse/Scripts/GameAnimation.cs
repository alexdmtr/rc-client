using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum AnimationTypes { Simple, FadeOut };

public class GameAnimation
{
    Transform AnimatedObject;
	AnimationTypes Type;
	
	Vector3 TargetPosition;
	Quaternion TargetRotation;
	
	float TimeLeft;
	
	public GameAnimation(Transform AnimatedObject, AnimationTypes Type, Vector3 TargetPosition, Quaternion TargetRotation, float TimeLeft)
	{
		this.AnimatedObject = AnimatedObject;
		this.Type = Type;
		this.TargetPosition = TargetPosition;
		this.TargetRotation = TargetRotation;
		this.TimeLeft = TimeLeft;
	}



	public bool IsRunning()
	{
		return TimeLeft >= 0;
	}


	public void Continue()
    {
        if (IsRunning())
        {
            float deltaTime = Time.deltaTime;
            float part = deltaTime / TimeLeft;
            AnimatedObject.position = Global.InterpolatePosition(AnimatedObject.position, TargetPosition, part);
            AnimatedObject.rotation = Global.InterpolateRotation(AnimatedObject.rotation, TargetRotation, part);

            TimeLeft -= deltaTime;

            if (Type == AnimationTypes.FadeOut)
            {

            }
        }


	}

	public GameAnimation Copy()
	{
		return new GameAnimation(AnimatedObject, Type, TargetPosition, TargetRotation, TimeLeft);
	}
	
}

public class AnimationChain
{
	public List<List<GameAnimation> > Chain = new List<List<GameAnimation>>();
	
	public AnimationChain()
	{
	}

	public AnimationChain(GameAnimation gameAnimation)
	{
		Enqueue(gameAnimation);
	}
	
	public bool IsRunning()
	{
		return Chain.Count > 0;
	}

	public void Enqueue(GameAnimation NextAnimation)
	{
		List<GameAnimation> l = new List<GameAnimation>();
		l.Add(NextAnimation);
		Enqueue(l);
	}
	public void Enqueue(List<GameAnimation> SynchronousAnimations)
	{
		Chain.Add(SynchronousAnimations);
	}
	
	public void Continue()
	{
		if (IsRunning())
		{

			foreach (GameAnimation anim in Chain[0])
				anim.Continue();
			Chain[0] = Chain[0].FindAll(x => x.IsRunning());
			if (Chain[0].Count == 0)
				Chain.RemoveAt(0);
		}
	}


}