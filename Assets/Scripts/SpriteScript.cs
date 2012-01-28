using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//DO NOT INHERIT.
public class SpriteScript : MonoBehaviour {

	//Constants.
	public enum AnimationType { Loop, OneShot, StopOnLastFrame };
	const float m_defaultFadeSpeed = 0.5f;

	//Editor variables.
	public Texture m_texture;
	public bool m_isBackground;
	public string m_charName;
	public string m_defaultAnimation;
	public bool m_randomOffset;
	public bool m_dontAutoScale;
	
	//Private variables.
	Dictionary<string, Animation> m_animations = new Dictionary<string, Animation>();
	Animation m_activeAnimation = null;
	float m_fadeSpeed;

	// Use this for initialization
	void Awake() {
		MakeSprite();
		PlayDefaultAnimation();
	}

	//Called when this will be destroyed.
	void OnDestroy() {
		foreach(Animation animation in m_animations.Values) {
			animation.Destroy();
		}
		m_animations.Clear();
	}

	// Update is called once per frame
	void Update() {
		if(m_activeAnimation != null) m_activeAnimation.Update(gameObject);

		//Update fading in/out of the sprite.
		if(m_fadeSpeed != 0.0f) {
			Color color = GetComponent<MeshRenderer>().material.color;
			float alpha = color.a + m_fadeSpeed * Time.deltaTime;
			if(alpha < 0.0f) {
				alpha = 0.0f;
				m_fadeSpeed = 0.0f;
				gameObject.active = false;
			} else if(alpha > 1.0f) {
				alpha = 1.0f;
				m_fadeSpeed = 0.0f;
			}
			GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, alpha);
		}
	}

	public void MakeSprite() {
		Texture texture = m_texture;
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if(meshRenderer.sharedMaterial == null) {
			meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);
			meshRenderer.sharedMaterial.mainTexture = texture;
		} else {
			gameObject.renderer.material.mainTexture = m_texture;
			return;
		}

		if(texture) {
			float scale = Camera.mainCamera.orthographicSize * 2.0f;
			float scaleX = texture.width * scale / 1024.0f;
			float scaleY = texture.height * scale / 1024.0f;
			transform.localScale = new Vector3(Mathf.Sign(transform.localScale.x) * scaleX, scaleY, 1.0f);
		}
	}

	public void SetAutoScale(bool autoScale) {
		m_dontAutoScale = !autoScale;
	}

	public void PlayDefaultAnimation() {
		if(m_defaultAnimation.Length > 0) {
			GetAnimation(m_defaultAnimation).Play();
			if(m_randomOffset) GetAnimation(m_defaultAnimation).SetRandomOffset();
		}
	}

	public Animation GetAnimation(string animationName) {
		Animation animation;
		m_animations.TryGetValue(animationName, out animation);
		if(animation == null) {
			animation = new Animation(this, m_charName + "/" + animationName);
			m_animations.Add(animationName, animation);
		}
		return animation;
	}

	public void ClearAnimations() {
		OnDestroy();
		m_activeAnimation = null;
	}

	//Make the gameObject face left if sign < 0.0f, right otherwise.
	public static void Face(GameObject gameObject, float sign){
		Vector3 scale = gameObject.transform.localScale;
		scale.x = Mathf.Sign(sign) * Mathf.Abs(scale.x);
		gameObject.transform.localScale = scale;
	}

	//Fade the character in/out.
	public void Fade(bool fadeIn) {
		if(fadeIn) {
			m_fadeSpeed = m_defaultFadeSpeed;
			gameObject.active = true;
		} else {
			m_fadeSpeed = -m_defaultFadeSpeed;
		}
	}

	//[System.Serializable]
	public class Animation {

		const float m_defaultFps = 12.0f;
		const float m_defaultSpf = 1.0f / m_defaultFps;

		public delegate void AnimationFinished(Animation animation);

		static Dictionary<string, PreloadedFrames> m_preloadedFrames;
		SpriteScript m_sprite;
		Texture[] m_frames;
		AnimationType m_animationType;
		Animation m_nextAnimation;//used for OneShot animations.
		AnimationFinished m_animFinishedCallback;
		string m_resourcePath;
		float m_length;//total length of the animation (in seconds)
		float m_offset;//offset into animation (in seconds)
		float m_fps, m_spf;
		bool m_backward;

		public Animation(SpriteScript sprite, string resourcePath) {
			m_sprite = sprite;
			m_resourcePath = resourcePath;

			//Try to find preloaded frames. Otherwise load them.
			if(m_preloadedFrames == null) m_preloadedFrames = new Dictionary<string, PreloadedFrames>(100);
			PreloadedFrames preloadedFrames;
			//print("Trying to load animation: " + m_resourcePath);
			m_preloadedFrames.TryGetValue(m_resourcePath, out preloadedFrames);
			if(preloadedFrames == null) {
				preloadedFrames = new PreloadedFrames();
				Object[] resources = Resources.LoadAll(m_resourcePath, typeof(Texture));
				preloadedFrames.m_textures = new Texture[resources.Length];
				resources.CopyTo(preloadedFrames.m_textures, 0);
				preloadedFrames.m_useCount = 0;
				m_preloadedFrames.Add(m_resourcePath, preloadedFrames);
				//print("Animation not found. Created new.");
			} else {
				//print("Animation found with " + preloadedFrames.m_useCount + " uses and " + preloadedFrames.m_textures.Length + " frames.");
			}

			preloadedFrames.m_useCount++;
			m_frames = preloadedFrames.m_textures;
			m_animationType = AnimationType.Loop;
			SetFpsFactor(1.0f);
			if(m_length <= 0.0f) {
				Debug.Log("0 length animation: " + resourcePath);
			}
		}

		public void Destroy() {
			//print("Trying to destroy animation: " + m_resourcePath);
			m_preloadedFrames[m_resourcePath].m_useCount--;
			if(m_preloadedFrames[m_resourcePath].m_useCount == 0) {
				//print("Destroying animation: " + m_resourcePath);
				m_preloadedFrames.Remove(m_resourcePath);
			} else {
				//print("Decreased uses to " + m_preloadedFrames[m_resourcePath].m_useCount);
			}
		}

		public Animation SetFpsFactor(float factor) {
			m_fps = factor * m_defaultFps;
			m_spf = 1.0f / m_fps;
			m_length = m_frames.Length * m_spf;
			return this;
		}

		public Animation SetAnimationFinishedCallback(AnimationFinished animFinishedCallback) {
			m_animFinishedCallback = animFinishedCallback;
			return this;
		}

		public Animation SetNextAnimation(Animation nextAnimation) {
			m_nextAnimation = nextAnimation;
			return this;
		}

		public Animation SetNextAnimation(string nextAnimation) {
			return SetNextAnimation(m_sprite.GetAnimation(nextAnimation));
		}

		public bool IsActive() {
			return m_sprite.m_activeAnimation == this;
		}

		public Animation Play() {
			return Play(AnimationType.Loop);
		}

		public void SetRandomOffset() {
			m_offset = Random.value * m_length;
		}

		public Animation Play(AnimationType animationType) {
			m_animationType = animationType;
			if(IsActive()) return this;
			m_offset = 0.0f;
			m_sprite.m_activeAnimation = this;
			m_backward = false;
			return this;
		}

		//Set this animation to play backward.
		public Animation Backward() {
			m_backward = true;
			m_offset = m_length;
			return this;
		}

		public void Update(GameObject gameObject) {
			if(m_length <= 0.0f) return;

			if(m_backward) {
				m_offset -= Time.deltaTime;
			} else {
				m_offset += Time.deltaTime;
			}
			bool animationDone = (!m_backward && m_offset >= m_length) || (m_backward && m_offset <= 0.0f);
			if(m_animationType == AnimationType.Loop) {
				if(m_backward) {
					while(m_offset >= 0.0f) {
						m_offset += m_length;
						if(m_animFinishedCallback != null) m_animFinishedCallback(this);
					}
				} else {
					while(m_offset >= m_length) {
						m_offset -= m_length;
						if(m_animFinishedCallback != null) m_animFinishedCallback(this);
					}
				}
			} else if(m_animationType == AnimationType.OneShot && animationDone) {
				if(m_nextAnimation == null) m_nextAnimation = m_sprite.GetAnimation(m_sprite.m_defaultAnimation);
				m_nextAnimation.Play();
				if(m_animFinishedCallback != null) m_animFinishedCallback(this);
				return;
			} else if(m_animationType == AnimationType.StopOnLastFrame && animationDone) {
				if(m_animFinishedCallback != null) m_animFinishedCallback(this);
			}
			int frame = Mathf.Min((int)(m_offset * m_fps), m_frames.Length - 1);
			gameObject.renderer.material.mainTexture = m_frames[frame];

			if(!m_sprite.m_dontAutoScale) {
				float scale = Camera.mainCamera.orthographicSize * 2.0f;
				float scaleX = gameObject.renderer.material.mainTexture.width * scale / 1024.0f;
				float scaleY = gameObject.renderer.material.mainTexture.height * scale / 1024.0f;
				if(scaleX != Mathf.Abs(gameObject.transform.localScale.x) || scaleY != Mathf.Abs(gameObject.transform.localScale.y)) {
					gameObject.transform.localScale = new Vector3(Mathf.Sign(gameObject.transform.localScale.x) * scaleX, scaleY, 1.0f);
				}
			}
		}

		class PreloadedFrames {

			public Texture[] m_textures;
			public int m_useCount;
		}
	}
}
