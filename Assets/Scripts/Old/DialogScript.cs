using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using Tools;

public enum Responses
{
	None,
	Left,
	Right
};

public enum Styles
{
	Undertale,
	Phylactery
};

public class DialogScript : MonoBehaviour
{
	private GameObject BlackAndWhiteBody;
	private GameObject PhylacteryBody;
	private Transform myTransform;
	private Transform myBox;
	private Transform TextTransform;

	public Sprite CharacterSprite;
	public Transform CharacterBody;

	private Image CharaImage;
	private Image Arrow;

	[HideInInspector]
	public RectTransform myRect;
	private RectTransform CharaRect;
	private RectTransform TipRect;

	private RectTransform[] LetterRect;
	private RectTransform[] UsedRect;
	private int used_index = 0;

	private IEnumerator sprite_coroutine;
	private IEnumerator body_coroutine;
	private IEnumerator movement_coroutine;

	private Quaternion charaTargetRotation = Quaternion.identity;

	[HideInInspector]
	public TextAsset myText;
	private string[] source;
	private string[] words;

	private Text[] Letter;
	private string line;

	[HideInInspector]
	public Responses Answer = Responses.None;

	private bool BodySpeak = false;
	private bool Movementable = true;
	private bool is_active = false;
	public bool Active
	{
		set
		{
			is_active = value;
			myBoxSetActive(value);
		}
		get
		{
			return is_active;
		}
	}

	private Styles Bodymode = Styles.Undertale;

	[HideInInspector]
	public int NbLineRead = 0;
	[HideInInspector]
	public int Value = 0;
	private int index = 0;
	private int ask_index = 0;
	private int Line_Length = 30;
	private int CharaAngle = -1;
	private float CharaOmega = 20.0f;
	private float CharaWideAngle = 6.0f;

	void Awake()
	{
		myTransform = transform;
		myBox = myTransform.Find("Box");

		BlackAndWhiteBody = myBox.Find("Bodies/Black&White").gameObject;
		PhylacteryBody = myBox.Find("Bodies/Phylactery").gameObject;

		BlackAndWhiteBody.SetActive(true);
		PhylacteryBody.SetActive(false);

		TextTransform = myBox.Find("Text");

		myRect = gameObject.GetComponent<RectTransform>();
		CharaRect = myBox.Find("CharacterMask/Character").GetComponent<RectTransform>();
		TipRect = myBox.Find("Bodies/Phylactery/Tip").GetComponent<RectTransform>();
		CharaImage = myBox.Find("CharacterMask/Character").GetComponent<Image>();

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		Arrow = myBox.Find("Arrow").GetComponent<Image>();
		Arrow.enabled = false;

		int length = TextTransform.childCount;
		Letter = new Text[length];
		LetterRect = new RectTransform[length];
		UsedRect = new RectTransform[length];
		for(int i = 0; i < length; i++)
		{
			Letter[i] = TextTransform.GetChild(i).GetComponent<Text>();
			LetterRect[i] = TextTransform.GetChild(i).GetComponent<RectTransform>();
			UsedRect[i] = null;
		}
	}

	void Start()
	{
		Active = false;
		source = myText.text.Split("\n"[0]);
		line = source[index];
	}

	public void Read(int nb_line, bool skip)
	{
		StartCoroutine(ReadCoroutine(nb_line, skip));
	}
	public void Read(int nb_line, bool skip, int font)
	{
		StartCoroutine(ReadCoroutine(nb_line, skip, font));
	}
	public void Read(int nb_line, float delay)
	{
		StartCoroutine(ReadCoroutine(nb_line, delay));
	}
	public void Read(int nb_line, float delay, int font)
	{
		StartCoroutine(ReadCoroutine(nb_line, delay, font));
	}
	public void Ask(int nb_line)
	{
		StartCoroutine(AskCoroutine(nb_line));
	}
	public void Ask(int nb_line, int font)
	{
		StartCoroutine(AskCoroutine(nb_line, font));
	}
	public void ClearUp()
	{
		int length = Letter.Length;
		for(int i = 0; i < length; i++)
			Letter[i].text = System.String.Empty;
	}
	public void Body(Styles mode)
	{
		int len = Letter.Length;
		Bodymode = mode;
		if(mode == Styles.Undertale)
		{
			PhylacteryBody.SetActive(false);
			BlackAndWhiteBody.SetActive(true);
			Arrow.color = Color.white;
			for(int i = 0; i < len; i++)
				Letter[i].color = Color.white;
		}
		else if(mode == Styles.Phylactery)
		{
			BlackAndWhiteBody.SetActive(false);
			PhylacteryBody.SetActive(true);
			Arrow.color = Color.black;
			for(int i = 0; i < len; i++)
				Letter[i].color = Color.black;
		}
	}
	public void Tip(Vector2 pos, Vector2 size, int rot)
	{
		TipRect.anchoredPosition = pos;
		TipRect.sizeDelta = size;
		TipRect.rotation = Quaternion.Euler(0, 0, rot);
	}
	public void Print(string s)
	{
		int key;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		ClearUp();
		key = 0;

		words = s.Split(" "[0]);
		words_length = words.Length;

		for(int m = 0; m < words_length; m++)
		{
			word = words[m];
			if(word.Contains("%d"))
				word = word.Replace("%d", Value.ToString());
			total_length = word.Length;

			if(word.Contains("<color="))
				length = total_length - ColorLength(word);
			else if(word.Contains(@"\n"))
				length = total_length - 3;
			else
				length = total_length;

			floor = key / Line_Length + 1;
			color = System.String.Empty;

			if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
			{
				key = floor * Line_Length;
				floor ++;
			}

			int l = 0;
			while(l < total_length)
			{
				if(word[l] == '\\' && word[l+1] == 'n')
				{
					key = floor * Line_Length;
					floor ++;
					l += 2;
				}
				else if(word[l] == '<' && word[l+1] == 'c')
				{
					int char_ind = 8;
					char c = word[char_ind-1];
					while(c != '>')
					{
						color = string.Concat(color, c.ToString());
						c = word[char_ind ++];
					}
					l += 8 + color.Length;
				}

				int alinea = key % Line_Length;
				if(word[l] != '>' && word[l] != '*' && alinea < 2 && Bodymode == Styles.Undertale)
					key += 2 - alinea;
				
				Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

				l++;
			}

			if(key % Line_Length != 0)
				key++;
		}
	}
	public void GoTo(int number)
	{
		index = (number - 1) * 5;
		line = source[index];

		string s = string.Concat("**index** ", number.ToString());

		while(!line.StartsWith(s))
		{
			if(index + 1 >= source.Length)
			{
				Debug.LogError("Error : No goto index found ! number = " + number.ToString());
				break;
			}
			line = source[++index];
		}

		line = source[++index];		//il y a un espace entre le "**index**" et le texte
	}
	public void Open()
	{
		FontDialogSetup();
		Active = true;
	}
	public void Close()
	{
		Active = false;
		ClearUp();
		if(movement_coroutine != null)
		{
			StopCoroutine(movement_coroutine);
			movement_coroutine = null;
		}
	}
	public void Close(int nb_line)
	{
		StartCoroutine(CloseCoroutine(nb_line));
	}
	public bool Enter()
	{
		return (Input.GetKeyDown("return") || Input.GetKeyDown("space") || Input.GetKeyDown("z"));
	}
	public void Linear(int rand, float speed)
	{
		if(movement_coroutine != null)
		{
			Movementable = false;
			StopCoroutine(movement_coroutine);
		}
		Movementable = true;
		movement_coroutine = LinearMovement(rand, speed);
		StartCoroutine(movement_coroutine);
	}
	public void Wave(float smooth, float interval)
	{
		if(movement_coroutine != null)
		{
			Movementable = false;
			StopCoroutine(movement_coroutine);
		}
		Movementable = true;
		movement_coroutine = WaveMovement(smooth, interval);
		StartCoroutine(movement_coroutine);
	}
	public void Square(int rand, float speed)
	{
		if(movement_coroutine != null)
		{
			Movementable = false;
			StopCoroutine(movement_coroutine);
		}
		Movementable = true;
		movement_coroutine = SquareMovement(rand, speed);
		StartCoroutine(movement_coroutine);
	}
	public void Revert(int rand)
	{
		if(movement_coroutine != null)
		{
			Movementable = false;
			StopCoroutine(movement_coroutine);
		}
		Movementable = true;
		movement_coroutine = RevertMovement(rand);
		StartCoroutine(movement_coroutine);
	}

	public void FontDialogSetup()
	{
		float dist_between_letter = 45f;
		float dist_between_character_x = (CharacterSprite == null) ? 100f : 500f;	//standard: 100f pour une font de 80
		int font = 90;

		float dist_x = (dist_between_letter/76f) * font;
		float dist_y = (25f/18f) * font;

		float edge_x = ((1f/1020f) * font + (47f/51f)) * dist_between_character_x;
		float edge_y = (-25f/18f) * font;

		float bound_x = 930 + (myRect.sizeDelta.x - 500) * 2;

		int i = 0;
		int j = 0;
		int indice = 0;
		int length = LetterRect.Length;

		while(indice < length)
		{
			LetterRect[indice].anchoredPosition = new Vector2(edge_x + dist_x * j, edge_y - dist_y * i);
			Letter[indice].fontSize = font;

			j++;
			indice ++;

			if(edge_x + dist_x * j > bound_x + 1f)
			{
				Line_Length = j;
				j = 0;
				i++;
			}
		}
	}
	public void FontDialogSetup(int font)
	{
		float dist_between_letter = 45f;
		float dist_between_character_x = (CharacterSprite == null) ? 100f : 500f;	//standard: 100f pour une font de 80

		int length = LetterRect.Length;

		float dist_x = (dist_between_letter/76f) * font;
		float dist_y = (25f/18f) * font;

		float edge_x = ((1f/1020f) * font + (47f/51f)) * dist_between_character_x;
		float edge_y = (-25f/18f) * font;

		float bound_x = 930 + (myRect.sizeDelta.x - 500) * 2;

		int i = 0;
		int j = 0;
		int indice = 0;

		while(indice < length)
		{
			LetterRect[indice].anchoredPosition = new Vector2(edge_x + dist_x * j, edge_y - dist_y * i);
			Letter[indice].fontSize = font;

			j++;
			indice ++;

			if(edge_x + dist_x * j > bound_x + 1f)
			{
				Line_Length = j;
				j = 0;
				i++;
			}
		}
	}
	public void FontDialogSetup(int font, float dist_between_letter)
	{
		float dist_between_character_x = (CharacterSprite == null) ? 100f : 500f;	//standard: 100f pour une font de 80

		int length = LetterRect.Length;
		
		float dist_x = (dist_between_letter/76f) * font;
		float dist_y = (25f/18f) * font;

		float edge_x = ((1f/1020f) * font + (47f/51f)) * dist_between_character_x;
		float edge_y = (-25f/18f) * font;

		float bound_x = 930 + (myRect.sizeDelta.x - 500) * 2;

		int i = 0;
		int j = 0;
		int indice = 0;

		while(indice < length)
		{
			LetterRect[indice].anchoredPosition = new Vector2(edge_x + dist_x * j, edge_y - dist_y * i);
			Letter[indice].fontSize = font;

			j++;
			indice ++;

			if(edge_x + dist_x * j > bound_x + 1f)
			{
				Line_Length = j;
				j = 0;
				i++;
			}
		}
	}
	public void FontDialogSetup(int font, float dist_between_letter, Vector2 anchor)
	{
		if(dist_between_letter < 0f)
			dist_between_letter = 45f;

		int length = LetterRect.Length;
		
		float dist_x = (dist_between_letter/76f) * font;
		float dist_y = (25f/18f) * font;

		float edge_x = anchor.x;
		float edge_y = anchor.y;

		float bound_x = 930 + (myRect.sizeDelta.x - 500) * 2;

		int i = 0;
		int j = 0;
		int indice = 0;

		while(indice < length)
		{
			LetterRect[indice].anchoredPosition = new Vector2(edge_x + dist_x * j, edge_y - dist_y * i);
			Letter[indice].fontSize = font;

			j++;
			indice ++;

			if(edge_x + dist_x * j > bound_x + 1f)
			{
				Line_Length = j;
				j = 0;
				i++;
			}
		}
	}
	public void SetRectTransform(Vector2 pos, Vector2 size)
	{
		myRect.anchoredPosition = pos;
		myRect.sizeDelta = size;
		FontDialogSetup();
	}
	public void SetPosition(Vector2 pos)
	{
		myRect.anchoredPosition = pos;
	}
	public void SetSize(Vector2 size)
	{
		myRect.sizeDelta = size;
		FontDialogSetup();
	}






	private IEnumerator ReadCoroutine(int nb_line, bool skip)
	{
		NbLineRead = 0;

		int key;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		if(CharaImage.enabled)
		{
			sprite_coroutine = SpriteSpeaker();
			StartCoroutine(sprite_coroutine);
		}
		if(CharacterBody != null)
		{
			body_coroutine = BodySpeaker();
			StartCoroutine(body_coroutine);
		}

		for(int i = 0; i < nb_line; i++)
		{
			ClearUp();
			key = 0;

			line = source[++index];
			words = line.Split(" "[0]);
			words_length = words.Length;

			for(int m = 0; m < words_length; m++)
			{
				word = words[m];
				if(word.Contains("%d"))
					word = word.Replace("%d", Value.ToString());
				total_length = word.Length;

				if(word.Contains("<color="))
					length = total_length - ColorLength(word);
				else if(word.Contains(@"\n"))
					length = total_length - 3;
				else
					length = total_length;

				floor = key / Line_Length + 1;
				color = System.String.Empty;

				if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
				{
					key = floor * Line_Length;
					floor ++;
				}

				int l = 0;
				while(l < total_length)
				{
					if(word[l] == '\\' && word[l+1] == 'n')
					{
						key = floor * Line_Length;
						floor ++;
						l += 2;
					}
					else if(word[l] == '<' && word[l+1] == 'c')
					{
						int char_ind = 8;
						char c = word[char_ind-1];
						while(c != '>')
						{
							color = string.Concat(color, c.ToString());
							c = word[char_ind ++];
						}
						l += 8 + color.Length;
					}

					int alinea = key % Line_Length;
					if(word[l] != '>' && word[l] != '*' && alinea < 2 && Bodymode == Styles.Undertale)
						key += 2 - alinea;
					
					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

					if(!(Enter() && skip))
					{
						float wait;
						float clock;

						if(word[l] == ',')
						{
							wait = 0.3f;
						}
						else if((word[l] == '.' || word[l] == '!' || word[l] == '?') 
								&& ((m != words_length-1 && l == total_length-1) || (l < total_length-1 && word[l+1] == '\\')))
						{
							wait = 0.5f;
							charaTargetRotation = Quaternion.identity;
						}
						else
						{
							wait = 0.0325f;
							if(key % 3 == 0)
							{
								CharaAngle -= CharaAngle * 2;
								charaTargetRotation = Quaternion.Euler(0, 0, CharaWideAngle * CharaAngle);
								BodySpeak = true;
							}
						}
						
						clock = 0.0f;
						while(clock < wait && !(Enter() && skip))
						{
							clock += Time.unscaledDeltaTime;
							yield return null;
						}
					}

					l++;
				}

				if(key % Line_Length != 0)
					key++;
			}

			charaTargetRotation = Quaternion.identity;
			Arrow.enabled = (Bodymode == Styles.Undertale);
			do
			{
				yield return null;
			}
			while(!Enter());
			Arrow.enabled = false;

			yield return new WaitForSecondsRealtime(0.1f);

			NbLineRead ++;
		}

		if(sprite_coroutine != null)
		{
			StopCoroutine(sprite_coroutine);
			CharaRect.rotation = Quaternion.identity;
			sprite_coroutine = null;
		}
		if(body_coroutine != null)
		{
			StopCoroutine(body_coroutine);
			CharacterBody.localScale = Vector3.one;
			CharacterBody.localPosition = Vector3.zero;
			body_coroutine = null;
		}
	}
	private IEnumerator ReadCoroutine(int nb_line, bool skip, int font)
	{
		NbLineRead = 0;

		int key;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		FontDialogSetup(font);
		if(CharaImage.enabled)
		{
			sprite_coroutine = SpriteSpeaker();
			StartCoroutine(sprite_coroutine);
		}
		if(CharacterBody != null)
		{
			body_coroutine = BodySpeaker();
			StartCoroutine(body_coroutine);
		}

		for(int i = 0; i < nb_line; i++)
		{
			ClearUp();
			key = 0;

			line = source[++index];
			words = line.Split(" "[0]);
			words_length = words.Length;

			for(int m = 0; m < words_length; m++)
			{
				word = words[m];
				if(word.Contains("%d"))
					word = word.Replace("%d", Value.ToString());
				total_length = word.Length;
				
				if(word.Contains("<color="))
					length = total_length - ColorLength(word);
				else if(word.Contains(@"\n"))
					length = total_length - 3;
				else
					length = total_length;

				floor = key / Line_Length + 1;
				color = System.String.Empty;

				if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
				{
					key = floor * Line_Length;
					floor ++;
				}

				int l = 0;
				while(l < total_length)
				{
					if(word[l] == '\\' && word[l+1] == 'n')
					{
						key = floor * Line_Length;
						floor ++;
						l += 2;
					}
					else if(word[l] == '<' && word[l+1] == 'c')
					{
						int char_ind = 8;
						char c = word[char_ind-1];
						while(c != '>')
						{
							color = string.Concat(color, c.ToString());
							c = word[char_ind ++];
						}
						l += 8 + color.Length;
					}

					int alinea = key % Line_Length;
					if(word[l] != '>' && word[l] != '*' && alinea < 2 && Bodymode == Styles.Undertale)
						key += 2 - alinea;

					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

					if(!(Enter() && skip))
					{
						float wait;
						float clock;

						if(word[l] == ',')
						{
							wait = 0.3f;
						}
						else if((word[l] == '.' || word[l] == '!' || word[l] == '?') 
								&& ((m != words_length-1 && l == total_length-1) || (l < total_length-1 && word[l+1] == '\\')))
						{
							wait = 0.5f;
							charaTargetRotation = Quaternion.identity;
						}
						else
						{
							wait = 0.0325f;
							if(key % 3 == 0)
							{
								CharaAngle -= CharaAngle * 2;
								charaTargetRotation = Quaternion.Euler(0, 0, 5 * CharaAngle);
								BodySpeak = true;
							}
						}
						
						clock = 0.0f;
						while(clock < wait && !(Enter() && skip))
						{
							clock += Time.unscaledDeltaTime;
							yield return null;
						}
					}

					l++;
				}

				if(key % Line_Length != 0)
					key++;
			}

			charaTargetRotation = Quaternion.identity;
			Arrow.enabled = (Bodymode == Styles.Undertale);
			do
			{
				yield return null;
			}
			while(!Enter());
			Arrow.enabled = false;

			yield return new WaitForSecondsRealtime(0.1f);

			NbLineRead ++;
		}

		if(sprite_coroutine != null)
		{
			StopCoroutine(sprite_coroutine);
			CharaRect.rotation = Quaternion.identity;
			sprite_coroutine = null;
		}
		if(body_coroutine != null)
		{
			StopCoroutine(body_coroutine);
			CharacterBody.localScale = Vector3.one;
			CharacterBody.localPosition = Vector3.zero;
			body_coroutine = null;
		}

		//FontDialogSetup();
	}
	private IEnumerator ReadCoroutine(int nb_line, float delay)
	{
		NbLineRead = 0;

		int key;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		if(CharaImage.enabled)
		{
			sprite_coroutine = SpriteSpeaker();
			StartCoroutine(sprite_coroutine);
		}
		if(CharacterBody != null)
		{
			body_coroutine = BodySpeaker();
			StartCoroutine(body_coroutine);
		}

		for(int i = 0; i < nb_line; i++)
		{
			ClearUp();
			key = 0;

			line = source[++index];
			words = line.Split(" "[0]);
			words_length = words.Length;

			for(int m = 0; m < words_length; m++)
			{
				word = words[m];
				if(word.Contains("%d"))
					word = word.Replace("%d", Value.ToString());
				total_length = word.Length;
				
				if(word.Contains("<color="))
					length = total_length - ColorLength(word);
				else if(word.Contains(@"\n"))
					length = total_length - 3;
				else
					length = total_length;

				floor = key / Line_Length + 1;
				color = System.String.Empty;

				if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
				{
					key = floor * Line_Length;
					floor ++;
				}

				int l = 0;
				while(l < total_length)
				{
					if(word[l] == '\\' && word[l+1] == 'n')
					{
						key = floor * Line_Length;
						floor ++;
						l += 2;
					}
					else if(word[l] == '<' && word[l+1] == 'c')
					{
						int char_ind = 8;
						char c = word[char_ind-1];
						while(c != '>')
						{
							color = string.Concat(color, c.ToString());
							c = word[char_ind ++];
						}
						l += 8 + color.Length;
					}

					int alinea = key % Line_Length;
					if(word[l] != '>' && word[l] != '*' && alinea < 2 && Bodymode == Styles.Undertale)
						key += 2 - alinea;

					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

					float wait;
					float clock;

					if(word[l] == ',')
					{
						wait = 0.3f;
					}
					else if((word[l] == '.' || word[l] == '!' || word[l] == '?') 
							&& ((m != words_length-1 && l == total_length-1) || (l < total_length-1 && word[l+1] == '\\')))
					{
						wait = 0.5f;
						charaTargetRotation = Quaternion.identity;
					}
					else
					{
						wait = 0.0325f;
						if(key % 3 == 0)
						{
							CharaAngle -= CharaAngle * 2;
							charaTargetRotation = Quaternion.Euler(0, 0, 5 * CharaAngle);
							BodySpeak = true;
						}
					}
					
					clock = 0.0f;
					while(clock < wait)
					{
						clock += Time.unscaledDeltaTime;
						yield return null;
					}

					l++;
				}

				if(key % Line_Length != 0)
					key++;
			}

			charaTargetRotation = Quaternion.identity;

			yield return new WaitForSecondsRealtime(delay);

			NbLineRead ++;
		}

		if(sprite_coroutine != null)
		{
			StopCoroutine(sprite_coroutine);
			CharaRect.rotation = Quaternion.identity;
			sprite_coroutine = null;
		}
		if(body_coroutine != null)
		{
			StopCoroutine(body_coroutine);
			CharacterBody.localScale = Vector3.one;
			CharacterBody.localPosition = Vector3.zero;
			body_coroutine = null;
		}
	}
	private IEnumerator ReadCoroutine(int nb_line, float delay, int font)
	{
		NbLineRead = 0;

		int key;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		FontDialogSetup(font);
		if(CharaImage.enabled)
		{
			sprite_coroutine = SpriteSpeaker();
			StartCoroutine(sprite_coroutine);
		}
		if(CharacterBody != null)
		{
			body_coroutine = BodySpeaker();
			StartCoroutine(body_coroutine);
		}

		for(int i = 0; i < nb_line; i++)
		{
			ClearUp();
			key = 0;

			line = source[++index];
			words = line.Split(" "[0]);
			words_length = words.Length;

			for(int m = 0; m < words_length; m++)
			{
				word = words[m];
				if(word.Contains("%d"))
					word = word.Replace("%d", Value.ToString());
				total_length = word.Length;
				
				if(word.Contains("<color="))
					length = total_length - ColorLength(word);
				else if(word.Contains(@"\n"))
					length = total_length - 3;
				else
					length = total_length;

				floor = key / Line_Length + 1;
				color = System.String.Empty;

				if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
				{
					key = floor * Line_Length;
					floor ++;
				}

				int l = 0;
				while(l < total_length)
				{
					if(word[l] == '\\' && word[l+1] == 'n')
					{
						key = floor * Line_Length;
						floor ++;
						l += 2;
					}
					else if(word[l] == '<' && word[l+1] == 'c')
					{
						int char_ind = 8;
						char c = word[char_ind-1];
						while(c != '>')
						{
							color = string.Concat(color, c.ToString());
							c = word[char_ind ++];
						}
						l += 8 + color.Length;
					}



					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

					float wait;
					float clock;

					if(word[l] == ',')
					{
						wait = 0.3f;
					}
					else if((word[l] == '.' || word[l] == '!' || word[l] == '?') 
							&& ((m != words_length-1 && l == total_length-1) || (l < total_length-1 && word[l+1] == '\\')))
					{
						wait = 0.5f;
						charaTargetRotation = Quaternion.identity;
					}
					else
					{
						wait = 0.0325f;
						if(key % 3 == 0)
						{
							CharaAngle -= CharaAngle * 2;
							charaTargetRotation = Quaternion.Euler(0, 0, 5 * CharaAngle);
							BodySpeak = true;
						}
					}
					
					clock = 0.0f;
					while(clock < wait)
					{
						clock += Time.unscaledDeltaTime;
						yield return null;
					}

					l++;
				}

				if(key % Line_Length != 0)
					key++;
			}

			charaTargetRotation = Quaternion.identity;

			yield return new WaitForSecondsRealtime(delay);

			NbLineRead ++;
		}

		if(sprite_coroutine != null)
		{
			StopCoroutine(sprite_coroutine);
			CharaRect.rotation = Quaternion.identity;
			sprite_coroutine = null;
		}
		if(body_coroutine != null)
		{
			StopCoroutine(body_coroutine);
			CharacterBody.localScale = Vector3.one;
			CharacterBody.localPosition = Vector3.zero;
			body_coroutine = null;
		}

		//FontDialogSetup();
	}
	private IEnumerator AskCoroutine(int nb_line)
	{
		Answer = Responses.None;

		int key = 0;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		if(CharaImage.enabled)
		{
			sprite_coroutine = SpriteSpeaker();
			StartCoroutine(sprite_coroutine);
		}
		if(CharacterBody != null)
		{
			body_coroutine = BodySpeaker();
			StartCoroutine(body_coroutine);
		}

		ClearUp();

		ask_index = index;
		line = Asker(nb_line, true);

		words = line.Split(" "[0]);
		words_length = words.Length;

		for(int m = 0; m < words_length; m++)
		{
			word = words[m];
			if(word.Contains("%d"))
				word = word.Replace("%d", Value.ToString());

			total_length = word.Length;
			
			if(word.Contains("<color="))
				length = total_length - ColorLength(word);
			else if(word.Contains(@"\n"))
				length = total_length - 3;
			else
				length = total_length;

			floor = key / Line_Length + 1;
			color = System.String.Empty;

			if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
			{
				key = floor * Line_Length;
				floor ++;
			}

			if(total_length == 0)
			{
				key ++;
			}

			int l = 0;
			while(l < total_length)
			{
				if(word[l] == '\\' && word[l+1] == 'n')
				{
					key = floor * Line_Length;
					floor ++;
					l += 2;
				}
				else if(word[l] == '<' && word[l+1] == 'c')
				{
					int char_ind = 8;
					char c = word[char_ind-1];
					while(c != '>')
					{
						color = string.Concat(color, c.ToString());
						c = word[char_ind ++];
					}
					l += 8 + color.Length;
				}

				int alinea = key % Line_Length;
				if(word[l] != '>' && word[l] != '*' && word[l] != '/' && alinea < 2 && Bodymode == Styles.Undertale)
					key += 2 - alinea;

				if(word[l] == '/')
					Letter[key ++].text = " ";
				else
					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

				float wait;

				if(word[l] != '/')
				{
					if(word[l] == ',')
					{
						wait = 0.3f;
					}
					else if((word[l] == '.' || word[l] == '!' || word[l] == '?') 
							&& ((m != words_length-1 && l == total_length-1) || (l < total_length-1 && word[l+1] == '\\')))
					{
						wait = 0.5f;
						charaTargetRotation = Quaternion.identity;
					}
					else
					{
						wait = 0.0325f;
						if(key % 3 == 0)
						{
							CharaAngle -= CharaAngle * 2;
							charaTargetRotation = Quaternion.Euler(0, 0, 5 * CharaAngle);
							BodySpeak = true;
						}
					}
					
					yield return new WaitForSecondsRealtime(wait);
				}

				l++;
			}

			if(key % Line_Length != 0)
				key++;
		}

		charaTargetRotation = Quaternion.identity;

		bool left = true;
		while(Enter() == false)
		{
			if(Input.GetAxisRaw("Horizontal") == -1.0f && left == false)
			{
				left = true;
				line = Asker(nb_line, true);
				AskPrint(line);
			}
			else if(Input.GetAxisRaw("Horizontal") == 1.0f && left == true)
			{
				left = false;
				line = Asker(nb_line, false);
				AskPrint(line);
			}
			yield return null;
		}

		Answer = (left) ? Responses.Left : Responses.Right;
		ClearUp();

		if(sprite_coroutine != null)
		{
			StopCoroutine(sprite_coroutine);
			CharaRect.rotation = Quaternion.identity;
			sprite_coroutine = null;
		}
		if(body_coroutine != null)
		{
			StopCoroutine(body_coroutine);
			CharacterBody.localScale = Vector3.one;
			CharacterBody.localPosition = Vector3.zero;
			body_coroutine = null;
		}
	}
	private IEnumerator AskCoroutine(int nb_line, int font)
	{
		Answer = Responses.None;

		int key = 0;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		CharaImage.enabled = (CharacterSprite != null);
		CharaImage.sprite = CharacterSprite;

		if(CharaImage.enabled)
		{
			sprite_coroutine = SpriteSpeaker();
			StartCoroutine(sprite_coroutine);
		}
		if(CharacterBody != null)
		{
			body_coroutine = BodySpeaker();
			StartCoroutine(body_coroutine);
		}

		ClearUp();

		FontDialogSetup(font);

		ask_index = index;
		line = Asker(nb_line, true);

		words = line.Split(" "[0]);
		words_length = words.Length;

		for(int m = 0; m < words_length; m++)
		{
			word = words[m];
			if(word.Contains("%d"))
				word = word.Replace("%d", Value.ToString());

			total_length = word.Length;
			
			if(word.Contains("<color="))
				length = total_length - ColorLength(word);
			else if(word.Contains(@"\n"))
				length = total_length - 3;
			else
				length = total_length;

			floor = key / Line_Length + 1;
			color = System.String.Empty;

			if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
			{
				key = floor * Line_Length;
				floor ++;
			}

			if(total_length == 0)
			{
				key ++;
			}

			int l = 0;
			while(l < total_length)
			{
				if(word[l] == '\\' && word[l+1] == 'n')
				{
					key = floor * Line_Length;
					floor ++;
					l += 2;
				}
				else if(word[l] == '<' && word[l+1] == 'c')
				{
					int char_ind = 8;
					char c = word[char_ind-1];
					while(c != '>')
					{
						color = string.Concat(color, c.ToString());
						c = word[char_ind ++];
					}
					l += 8 + color.Length;
				}

				int alinea = key % Line_Length;
				if(word[l] != '>' && word[l] != '*' && word[l] != '/' && alinea < 2 && Bodymode == Styles.Undertale)
					key += 2 - alinea;

				if(word[l] == '/')
					Letter[key ++].text = " ";
				else
					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

				float wait;

				if(word[l] != '/')
				{
					if(word[l] == ',')
					{
						wait = 0.3f;
					}
					else if((word[l] == '.' || word[l] == '!' || word[l] == '?') 
							&& ((m != words_length-1 && l == total_length-1) || (l < total_length-1 && word[l+1] == '\\')))
					{
						wait = 0.5f;
						charaTargetRotation = Quaternion.identity;
					}
					else
					{
						wait = 0.0325f;
						if(key % 3 == 0)
						{
							CharaAngle -= CharaAngle * 2;
							charaTargetRotation = Quaternion.Euler(0, 0, 5 * CharaAngle);
							BodySpeak = true;
						}
					}
					
					yield return new WaitForSecondsRealtime(wait);
				}

				l++;
			}

			if(key % Line_Length != 0)
				key++;
		}

		charaTargetRotation = Quaternion.identity;

		bool left = true;
		while(Enter() == false)
		{
			if(Input.GetAxisRaw("Horizontal") == -1.0f && left == false)
			{
				left = true;
				line = Asker(nb_line, true);
				AskPrint(line);
			}
			else if(Input.GetAxisRaw("Horizontal") == 1.0f && left == true)
			{
				left = false;
				line = Asker(nb_line, false);
				AskPrint(line);
			}
			yield return null;
		}

		Answer = (left) ? Responses.Left : Responses.Right;
		ClearUp();

		if(sprite_coroutine != null)
		{
			StopCoroutine(sprite_coroutine);
			CharaRect.rotation = Quaternion.identity;
			sprite_coroutine = null;
		}
		if(body_coroutine != null)
		{
			StopCoroutine(body_coroutine);
			CharacterBody.localScale = Vector3.one;
			CharacterBody.localPosition = Vector3.zero;
			body_coroutine = null;
		}

		//FontDialogSetup();
	}

	private void myBoxSetActive(bool active)
	{
		Image[] images = GetComponentsInChildren<Image>();
		Text[] letters = GetComponentsInChildren<Text>();

		foreach(Image image in images)
			image.enabled = active;
		foreach(Text letter in letters)
			letter.enabled = active;
	}
	private IEnumerator CloseCoroutine(int nb_line)
	{
		while(nb_line != 0 && NbLineRead != nb_line)
			yield return null;

		Active = false;
		ClearUp();
	}
	private int ColorLength(string s)
	{
		int length = 0;
		while(s[length] != '>')
		{
			length ++;
		}
		return length + 1;
	}
	private void AskPrint(string s)
	{
		int key = 0;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		ClearUp();

		words = s.Split(" "[0]);
		words_length = words.Length;

		for(int m = 0; m < words_length; m++)
		{
			word = words[m];
			if(word.Contains("%d"))
				word = word.Replace("%d", Value.ToString());

			total_length = word.Length;
			
			if(word.Contains("<color="))
				length = total_length - ColorLength(word);
			else if(word.Contains(@"\n"))
				length = total_length - 3;
			else
				length = total_length;

			floor = key / Line_Length + 1;
			color = System.String.Empty;

			if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
			{
				key = floor * Line_Length;
				floor ++;
			}

			if(total_length == 0)
			{
				key ++;
			}

			int l = 0;
			while(l < total_length)
			{
				if(word[l] == '\\' && word[l+1] == 'n')
				{
					key = floor * Line_Length;
					floor ++;
					l += 2;
				}
				else if(word[l] == '<' && word[l+1] == 'c')
				{
					int char_ind = 8;
					char c = word[char_ind-1];
					while(c != '>')
					{
						color = string.Concat(color, c.ToString());
						c = word[char_ind ++];
					}
					l += 8 + color.Length;
				}

				int alinea = key % Line_Length;
				if(word[l] != '>' && word[l] != '*' && word[l] != '/' && alinea < 2 && Bodymode == Styles.Undertale)
					key += 2 - alinea;

				if(word[l] == '/')
					Letter[key ++].text = " ";
				else
					Letter[key ++].text = (string.IsNullOrEmpty(color)) ? word[l].ToString() : "<color=" + color + ">" + word[l].ToString() + "</color>";

				l++;
			}

			if(key % Line_Length != 0)
				key++;
		}
	}
	private bool Sameletter(string s)
	{
		int len = s.Length;
		int index;
		int letter_index = 0;
		int[] count = new int[len];
		char[] letters = new char[len];
		char letter;

		for(int i = 0; i < len; i++)
		{
			letter = s[i];

			if(letters.Contains(letter))
			{
				index = System.Array.IndexOf(letters, letter);
				count[index] ++;
				if(count[index] > 3)
				{
					return true;
				}
			}
			else
			{
				letters[letter_index] = letter;
				count[letter_index] = 1;
				letter_index ++;
			}
		}

		return false;
	}
	private string Asker(int nb_line, bool left)
	{
		string s;
		int space;
		int asker_index = ask_index;
		int right_dividende = Mathf.RoundToInt((3f/40f) * Line_Length);
		int left_dividende = Mathf.RoundToInt((16f/40f) * Line_Length);
		int question_length;

		s = source[++ asker_index];
		s += " ";
		question_length = 3 - (s.Length - 1)/Line_Length;
		space = question_length * Line_Length - Bounder(s);
		space += right_dividende;
		for(int k = 0; k < space; k++)
		{
			s += "/";
		}
		s += " ";
		s += (left) ? "<color=yellow>* <color=yellow>" : "/ ";
		s += source[++ asker_index];
		s += " ";
		space = left_dividende - right_dividende - source[asker_index].Length;
		if(space > 0)
		{
			for(int k = 0; k < space; k++)
				s += "/";
			s += " ";
		}
		s += (!left) ? "<color=yellow>* <color=yellow>" : "/ ";
		s += source[++ asker_index];

		int i = 3;
		while(i < nb_line)
		{
			s += " ";
			space = Line_Length - left_dividende - 6 - source[asker_index].Length + right_dividende;
			for(int k = 0; k < space; k++)
			{
				s += "/";
			}
			s += " ";
			s += (left) ? "<color=yellow>" + source[++ asker_index] : source[++ asker_index];

			i++;

			if(i < nb_line)
			{
				s += " ";
				space = left_dividende - right_dividende - source[asker_index].Length + 2;
				for(int k = 0; k < space; k++)
				{
					s += "/";
				}
				s += " ";
				s += (!left) ? "<color=yellow>" + source[++ asker_index] : source[++ asker_index];

				i++;
			}
		}
		
		return s;
	}
	private int Bounder(string s)
	{
		int key = 0;
		int floor = 1;
		int length = 0;
		int words_length;
		int total_length;

		string word;
		string color;

		words = s.Split(" "[0]);
		words_length = words.Length;

		for(int m = 0; m < words_length; m++)
		{
			word = words[m];
			if(word.Contains("%d"))
				word = word.Replace("%d", Value.ToString());
			total_length = word.Length;

			if(word.Contains("<color="))
				length = total_length - ColorLength(word);
			else if(word.Contains(@"\n"))
				length = total_length - 3;
			else
				length = total_length;

			floor = key / Line_Length + 1;
			color = System.String.Empty;

			if(key + length > floor * Line_Length && !word.Contains("!!!") && !word.Contains("....") && !Sameletter(word))
			{
				key = floor * Line_Length;
				floor ++;
			}

			int l = 0;
			while(l < total_length)
			{
				if(word[l] == '\\' && word[l+1] == 'n')
				{
					key = floor * Line_Length;
					floor ++;
					l += 2;
				}
				else if(word[l] == '<' && word[l+1] == 'c')
				{
					int char_ind = 8;
					char c = word[char_ind-1];
					while(c != '>')
					{
						color = string.Concat(color, c.ToString());
						c = word[char_ind ++];
					}
					l += 8 + color.Length;
				}

				int alinea = key % Line_Length;
				if(word[l] != '>' && word[l] != '*' && alinea < 2 && Bodymode == Styles.Undertale)
					key += 2 - alinea;
				
				key ++;
				l++;
			}

			if(key % Line_Length != 0)
				key++;
		}

		return key - (floor - 1) * Line_Length - 1;
	}
	private IEnumerator SpriteSpeaker()
	{
		while(CharaImage.enabled)
		{
			CharaRect.rotation = Quaternion.Slerp(CharaRect.rotation, charaTargetRotation, CharaOmega * Time.deltaTime);
			yield return null;
		}
	}
	private IEnumerator BodySpeaker()
	{
		float speakSpeed = 2.0f;
		float downScale = 0.75f;
		Vector3 speakScale = new Vector3(1, downScale, 1);
		Vector3 speakLocalPosition = new Vector3(0, (downScale - 1)/2f, 0);
		
		while(CharacterBody != null)
		{
			if(BodySpeak)
			{
				BodySpeak = false;

				while(!(Vector3.Distance(CharacterBody.localScale, speakScale) < 0.05f && Vector3.Distance(CharacterBody.localPosition, speakLocalPosition) < 0.05f))
				{
					CharacterBody.localScale = Vector3.MoveTowards(CharacterBody.localScale, speakScale, speakSpeed * Time.deltaTime);
					CharacterBody.localPosition = Vector3.MoveTowards(CharacterBody.localPosition, speakLocalPosition, speakSpeed/2f * Time.deltaTime);
					yield return null;
				}
				while(!(Vector3.Distance(CharacterBody.localScale, Vector3.one) < 0.05f && Vector3.Distance(CharacterBody.localPosition, Vector3.zero) < 0.05f))
				{
					CharacterBody.localScale = Vector3.MoveTowards(CharacterBody.localScale, Vector3.one, speakSpeed * Time.deltaTime);
					CharacterBody.localPosition = Vector3.MoveTowards(CharacterBody.localPosition, Vector3.zero, speakSpeed/2f * Time.deltaTime);
					yield return null;
				}

				CharacterBody.localScale = Vector3.one;
				CharacterBody.localPosition = Vector3.zero;
			}
			else
			{
				yield return null;
			}
		}
	}



	
	private IEnumerator LinearMovement(int rand, float speed)
	{
		RectTransform itsRect;
		int choice;

		while(Movementable)
		{
			for(int i = 0; i < line.Length; i++)
			{
				itsRect = LetterRect[i];
				choice = Random.Range(0, rand);

				if(choice == 0 && !UsedRect.Contains(itsRect))
					StartCoroutine(LinearVertical(itsRect, speed));
				else if(choice == 1 && !UsedRect.Contains(itsRect))
					StartCoroutine(LinearHorizontal(itsRect, speed));
			}
			yield return null;
		}
	}
	private IEnumerator LinearVertical(RectTransform rect, float speed)
	{
		UsedRect[used_index ++] = rect;

		float posY = rect.anchoredPosition.y;
		float destY = posY + (Random.Range(0, 2) * 2 - 1) * 10.0f;

		while(rect.anchoredPosition.y != destY)
		{
			rect.SetAnchoredPositionY(Mathf.MoveTowards(rect.anchoredPosition.y, destY, speed * Time.unscaledDeltaTime));
			yield return null;
		}
		while(rect.anchoredPosition.y != posY)
		{
			rect.SetAnchoredPositionY(Mathf.MoveTowards(rect.anchoredPosition.y, posY, speed * Time.unscaledDeltaTime));
			yield return null;
		}

		UsedRect[System.Array.IndexOf(UsedRect, rect)] = null;
		used_index --;
	}
	private IEnumerator LinearHorizontal(RectTransform rect, float speed)
	{
		UsedRect[used_index ++] = rect;

		float posX = rect.anchoredPosition.x;
		float destX = posX + (Random.Range(0, 2) * 2 - 1) * 10.0f;

		while(rect.anchoredPosition.x != destX)
		{
			rect.SetAnchoredPositionX(Mathf.MoveTowards(rect.anchoredPosition.x, destX, speed * Time.unscaledDeltaTime));
			yield return null;
		}
		while(rect.anchoredPosition.x != posX)
		{
			rect.SetAnchoredPositionX(Mathf.MoveTowards(rect.anchoredPosition.x, posX, speed * Time.unscaledDeltaTime));
			yield return null;
		}

		UsedRect[System.Array.IndexOf(UsedRect, rect)] = null;
		used_index --;
	}

	private IEnumerator WaveMovement(float smooth, float interval)
	{
		RectTransform itsRect;
		int len = TextTransform.childCount;

		for(int i = 0; i < len; i++)
		{
			itsRect = LetterRect[i];
			StartCoroutine(WaveVertical(itsRect, smooth));
			yield return new WaitForSeconds(interval);
		}
	}
	private IEnumerator WaveVertical(RectTransform rect, float smooth)
	{
		float posY = rect.anchoredPosition.y;
		float dest1 = posY + 10.0f;
		float dest2 = posY - 10.0f;
		float jerk = 0.0f;

		while(rect.anchoredPosition.y < dest1 - 0.75f)
		{
			rect.SetAnchoredPositionY(Mathf.SmoothDamp(rect.anchoredPosition.y, dest1, ref jerk, smooth, Mathf.Infinity, Time.unscaledDeltaTime));
			yield return null;
		}

		while(Movementable)
		{
			while(rect.anchoredPosition.y > dest2 + 0.75f)
			{
				rect.SetAnchoredPositionY(Mathf.SmoothDamp(rect.anchoredPosition.y, dest2, ref jerk, smooth * 2.0f, Mathf.Infinity, Time.unscaledDeltaTime));
				yield return null;
			}
			while(rect.anchoredPosition.y < dest1 - 0.75f)
			{
				rect.SetAnchoredPositionY(Mathf.SmoothDamp(rect.anchoredPosition.y, dest1, ref jerk, smooth * 2.0f, Mathf.Infinity, Time.unscaledDeltaTime));
				yield return null;
			}
		}
	}

	private IEnumerator SquareMovement(int rand, float speed)
	{
		RectTransform itsRect;
		int choice;
		int len = TextTransform.childCount;

		for(int i = 0; i < len; i++)
		{
			itsRect = LetterRect[i];
			choice = Random.Range(0, rand);

			if(choice == 0)
				StartCoroutine(SquareVertical(itsRect, speed));
			else if(choice == 1)
				StartCoroutine(SquareHorizontal(itsRect, speed));

			if(i%(len/4) == 0)
				yield return null;
		}
	}
	private IEnumerator SquareVertical(RectTransform rect, float speed)
	{
		float posY = rect.anchoredPosition.y;
		float dest1 = posY + 5.0f;
		float dest2 = posY - 5.0f;

		while(Movementable)
		{
			while(rect.anchoredPosition.y != dest1)
			{
				rect.SetAnchoredPositionY(Mathf.MoveTowards(rect.anchoredPosition.y, dest1, speed * Time.unscaledDeltaTime));
				yield return null;
			}
			while(rect.anchoredPosition.y != dest2)
			{
				rect.SetAnchoredPositionY(Mathf.MoveTowards(rect.anchoredPosition.y, dest2, speed * Time.unscaledDeltaTime));
				yield return null;
			}
		}
	}
	private IEnumerator SquareHorizontal(RectTransform rect, float speed)
	{
		float posX = rect.anchoredPosition.x;
		float dest1 = posX + 5.0f;
		float dest2 = posX - 5.0f;

		while(Movementable)
		{
			while(rect.anchoredPosition.x != dest1)
			{
				rect.SetAnchoredPositionX(Mathf.MoveTowards(rect.anchoredPosition.x, dest1, speed * Time.unscaledDeltaTime));
				yield return null;
			}
			while(rect.anchoredPosition.x != dest2)
			{
				rect.SetAnchoredPositionX(Mathf.MoveTowards(rect.anchoredPosition.x, dest2, speed * Time.unscaledDeltaTime));
				yield return null;
			}
		}
	}

	private IEnumerator RevertMovement(int rand)
	{
		RectTransform itsRect;
		int choice;

		while(Movementable)
		{
			for(int i = 0; i < line.Length + 2; i++)
			{
				itsRect = TextTransform.GetChild(i + 2).GetComponent<RectTransform>();
				choice = Random.Range(0, rand);

				if(!UsedRect.Contains(itsRect))
				{
					if(choice == 0)
						StartCoroutine(RevertX(itsRect));
					else if(choice == 1)
						StartCoroutine(RevertY(itsRect));
					else if(choice == 2)
						StartCoroutine(RevertZ(itsRect));
				}
			}
			yield return new WaitForSecondsRealtime(1.0f);
		}
	}
	private IEnumerator RevertX(RectTransform rect)
	{
		UsedRect[used_index ++] = rect;
		rect.rotation *= Quaternion.Euler(180, 0, 0);

		yield return new WaitForSecondsRealtime(0.5f);

		rect.rotation = Quaternion.identity;
		UsedRect[System.Array.IndexOf(UsedRect, rect)] = null;
		used_index --;
	}
	private IEnumerator RevertY(RectTransform rect)
	{
		UsedRect[used_index ++] = rect;
		rect.rotation *= Quaternion.Euler(0, 180, 0);

		yield return new WaitForSecondsRealtime(0.5f);

		rect.rotation = Quaternion.identity;
		UsedRect[System.Array.IndexOf(UsedRect, rect)] = null;
		used_index --;
	}
	private IEnumerator RevertZ(RectTransform rect)
	{
		UsedRect[used_index ++] = rect;
		rect.rotation *= Quaternion.Euler(0, 0, 180);

		yield return new WaitForSecondsRealtime(0.5f);

		rect.rotation = Quaternion.identity;
		UsedRect[System.Array.IndexOf(UsedRect, rect)] = null;
		used_index --;
	}
}