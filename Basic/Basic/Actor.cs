using System;
using System.Collections.Generic;

namespace Basic.Framework
{
	public class Actor
	{
		public string Name{ get; set; }
		
		public enum ActorStatus
		{
			Action,
			UpdateOnly,
			RenderOnly,
			Rest,
			NoUse,
			Dead,
		}
		
		ActorStatus status;
		
		public ActorStatus Status {
			get { return status;}
			set {
				status = value;
				StatusNextFrame = value;
			}
		}
		
		public ActorStatus StatusNextFrame;
		protected Int32 level = 0;
		protected List<Actor> children = new List<Actor> ();
		
		public List<Actor> Children {
			get { return Children;}
		}
		
		public Actor ()
		{
			this.Name = "noName";
			this.Status = ActorStatus.Action;
		}

		public Actor (string name)
		{
			this.Name = name;
			this.Status = ActorStatus.Action;	
		}
		
		public override string ToString ()
		{
			return this.Name;
		}
		
		virtual public void Update ()
		{
			foreach (Actor actorChild in children) {
				if (actorChild.status == ActorStatus.Action || actorChild.status == ActorStatus.UpdateOnly)
					actorChild.Update ();
			}
		}

		virtual public void Render ()
		{
			foreach (Actor actorChild in children) {
				actorChild.Render ();
			}
			
		}

		virtual public void AddChild (Actor actor)
		{
			children.Add (actor);
			actor.level = this.level + 1;
		}
		
		virtual public Actor Search (string name)
		{
			if (this.Name == name)
				return this;
			
			Actor retActor;
			
			foreach (Actor actorChild in children) {
				if ((retActor = actorChild.GetActor (name)) != null)
					return retActor;
				
			}
			return null;
		}

		public Actor GetActor (string name)
		{
			if (this.Name == name)
				return this;

			Actor retActor;

			foreach (Actor actorChild in children) {
				if ((retActor = actorChild.GetActor (name)) != null)
					return retActor;
			}

			return null;
		}

		public Actor SearchByPath (string path)
		{
			int pos = path.IndexOf ('/');
			if (pos != -1) {
				string nameLeft = path.Substring (0, pos);
				string nameRight = path.Substring (pos + 1);
				
				foreach (Actor actorChild in this.Children) {
					if (actorChild.Name == nameLeft)
						return actorChild.SearchByPath (nameRight);
				}
			} else {
				
				foreach (Actor actorChild in this.Children) {
					if (actorChild.Name == path)
						return actorChild;
				}
			}
			
			return null;
		}

		virtual public void CheckStatus ()
		{
			if (this.status != this.StatusNextFrame)
				this.status = this.StatusNextFrame;
			
			
			//Set dead flags for all the children if the player is dead.
			if (this.Status == ActorStatus.Dead) {
				foreach (Actor actorChild in children) {
					actorChild.Status = ActorStatus.Dead;
				}
			}
			
			//Visit children with recursive call.
			foreach (Actor actorChild in children) {
				actorChild.CheckStatus ();
			}
			
			//@e Delete a child where the dead flag is set from a list.
			children.RemoveAll (CheckDeadActor);
		}
		
		static bool CheckDeadActor (Actor actor)
		{
			
		
			return actor.Status == ActorStatus.Dead; 
		}
	
		public Int32 GetAliveChildren ()
		{
			Int32 cnt = 0;

			foreach (Actor actorChild in children) {
				if (actorChild.Status != ActorStatus.Dead) {
					cnt++;
					cnt += actorChild.GetAliveChildren ();
				}
			}

			return cnt;
		}
		
		
	}
}

