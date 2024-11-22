using System.Collections.Generic;
using UnityEngine;

public class ClothingAnimationSync : MonoBehaviour
{
	public Animator baseBodyAnimator;
	public Animator baseAnimator;  // Animator bazowy postaci (np. cia³o)
	public SpriteRenderer clothingRenderer;  // SpriteRenderer dla ubrania

	// Dictionary do przechowywania animacji i odpowiadaj¹cych im sprite'ów
	private Dictionary<string, Sprite[]> animationSprites = new Dictionary<string, Sprite[]>();

	private string currentAnimationState;  // Bie¿¹cy stan animacji
	private string currentDirection;  // Bie¿¹cy kierunek
	private int currentFrame;  // Bie¿¹ca klatka w animacji

	public Sprite[] idleRSprites;
	public Sprite[] idleLSprites;
	public Sprite[] idleUSprites;
	public Sprite[] idleDSprites;

	public Sprite[] runRSprites;
	public Sprite[] runLSprites;
	public Sprite[] runUSprites;
	public Sprite[] runDSprites;

	public Sprite[] jumpSprites;
	public Sprite[] dashSprites;
	public Sprite[] deathSprites;
	public Sprite[] attackSword01Sprites;
	public Sprite[] attackSword02Sprites;
	public Sprite[] attackSword03Sprites;

	private void Start()
	{
		// Dodaj zestawy sprite'ów dla ka¿dej animacji (mo¿na je te¿ wczytaæ dynamicznie)
		animationSprites["Idle_01_R"] = idleRSprites;
		animationSprites["Idle_01_L"] = idleLSprites;
		animationSprites["Idle_01_U"] = idleUSprites;
		animationSprites["Idle_01_D"] = idleDSprites;

		animationSprites["Run_01_R"] = runRSprites;
		animationSprites["Run_01_L"] = runLSprites;
		animationSprites["Run_01_U"] = runUSprites;
		animationSprites["Run_01_D"] = runDSprites;

		animationSprites["Jump_01"] = jumpSprites;
		animationSprites["Jump_01"] = jumpSprites;
		animationSprites["Jump_01"] = jumpSprites;

		// Dodaj pozosta³e animacje zgodnie z nazwami stanu Animatora
		animationSprites["attack_Sword_01"] = attackSword01Sprites;
		animationSprites["attack_Sword_02"] = attackSword02Sprites;
		// itd.
	}
	private void Update()
	{
		// Aktualizacja wartoœci X i Y w baseAnimator na podstawie baseBodyAnimator
		GetDirectionFromAnimator();

		// Na podstawie wartoœci X i Y w baseAnimator okreœl kierunek
		string newDirection = DetermineDirectionFromXY();

		// Pobierz bie¿¹cy stan animacji i dodaj kierunek
		string newAnimationState = GetCurrentAnimationState() ;

		// Jeœli zmieni³ siê stan animacji lub kierunek, resetuj klatkê
		if (newAnimationState != currentAnimationState || newDirection != currentDirection)
		{
			currentAnimationState = newAnimationState;
			currentDirection = newDirection;
			currentFrame = 0; // Reset klatki przy zmianie animacji
		}

		// Aktualizuj sprite'a ubrania na podstawie bie¿¹cej klatki i kierunku
		UpdateClothingSprite();
	}

	private void GetDirectionFromAnimator()
	{
		// Pobierz wartoœci X i Y z Animatora bazowego cia³a
		float directionX = baseBodyAnimator.GetFloat("X");
		float directionY = baseBodyAnimator.GetFloat("Y");

		// Przeka¿ te wartoœci do Animatora dla ubrania
		baseAnimator.SetFloat("X", directionX);
		baseAnimator.SetFloat("Y", directionY);
	}

	private string DetermineDirectionFromXY()
	{
		// Pobierz wartoœci X i Y z Animatora
		float directionX = baseAnimator.GetFloat("X");
		float directionY = baseAnimator.GetFloat("Y");

		// Okreœl kierunek na podstawie wartoœci X i Y
		if (directionY > 0.5f)
			return "U";
		else if (directionY < -0.5f)
			return "D";
		else if (directionX > 0.5f)
			return "R";
		else
			return "L";
	}

	private string GetCurrentAnimationState()
	{
		// Pobiera nazwê aktualnego stanu animacji z Animatora
		AnimatorStateInfo stateInfo = baseAnimator.GetCurrentAnimatorStateInfo(0);
		return stateInfo.IsName("Idle_01") ? "Idle_01" :
				stateInfo.IsName("Run_01") ? "Run_01" :
				stateInfo.IsName("Jump_01") ? "Jump_01" :
				stateInfo.IsName("Dash_01") ? "Dash_01" :
				stateInfo.IsName("Death_01") ? "Death_01" :
				stateInfo.IsName("Attack_Sword_01") ? "Attack_Sword_01" :
				stateInfo.IsName("Attack_Sword_02") ? "Attack_Sword_02" :
				stateInfo.IsName("Attack_Sword_03") ? "Attack_Sword_03" :
				"Idle_01";  // Domyœlny stan, jeœli nic niepasuje
	}

	private void UpdateClothingSprite()
	{
		if (animationSprites.ContainsKey(currentAnimationState))
		{
			Sprite[] sprites = animationSprites[currentAnimationState];

			// Oblicz bie¿¹c¹ klatkê animacji na podstawie czasu
			AnimatorStateInfo stateInfo = baseAnimator.GetCurrentAnimatorStateInfo(0);
			float normalizedTime = stateInfo.normalizedTime % 1;  // Normalizuje czas
			currentFrame = Mathf.FloorToInt(normalizedTime * sprites.Length);

			// Podstaw sprite’a jeœli klatka mieœci siê w zakresie
			if (currentFrame < sprites.Length)
			{
				clothingRenderer.sprite = sprites[currentFrame];
			}
		}
	}
}
