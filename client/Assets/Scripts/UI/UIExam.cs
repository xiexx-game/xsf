//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\UI\UIExam.cs
// 作者：Xoen Xie
// 时间：9/1/2023
// 描述：
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class UIExam : UIBase
{
// UI_PROP_START
	public TextMeshProUGUI Question { get; private set; }	// 
	public TextMeshProUGUI Answer { get; private set; }	// 
	public GameObject BtnClose { get; private set; }	// 
	public GameObject Num0 { get; private set; }	// 
	public GameObject Num1 { get; private set; }	// 
	public GameObject Num2 { get; private set; }	// 
	public GameObject Num3 { get; private set; }	// 
	public GameObject Num4 { get; private set; }	// 
	public GameObject Num5 { get; private set; }	// 
	public GameObject Num6 { get; private set; }	// 
	public GameObject Num7 { get; private set; }	// 
	public GameObject Num8 { get; private set; }	// 
	public GameObject Num9 { get; private set; }	// 
	public GameObject Cancel { get; private set; }	// 
	public GameObject Enter { get; private set; }	// 
// UI_PROP_END

    public override string Name { get { return "UIExam"; } }

	private uint m_nCurrentLevel;

    public override void OnInit()
    {
        // UI_INIT_START
		// 
		Question = RootT.Find("question").GetComponent<TextMeshProUGUI>();
		// 
		Answer = RootT.Find("question/answer").GetComponent<TextMeshProUGUI>();
		// 
		BtnClose = RootT.Find("close").gameObject;
		UIEventClick.Set(BtnClose, OnBtnCloseClick);
		// 
		Num0 = RootT.Find("kebord/0").gameObject;
		UIEventClick.Set(Num0, OnNum0Click);
		// 
		Num1 = RootT.Find("kebord/1").gameObject;
		UIEventClick.Set(Num1, OnNum1Click);
		// 
		Num2 = RootT.Find("kebord/2").gameObject;
		UIEventClick.Set(Num2, OnNum2Click);
		// 
		Num3 = RootT.Find("kebord/3").gameObject;
		UIEventClick.Set(Num3, OnNum3Click);
		// 
		Num4 = RootT.Find("kebord/4").gameObject;
		UIEventClick.Set(Num4, OnNum4Click);
		// 
		Num5 = RootT.Find("kebord/5").gameObject;
		UIEventClick.Set(Num5, OnNum5Click);
		// 
		Num6 = RootT.Find("kebord/6").gameObject;
		UIEventClick.Set(Num6, OnNum6Click);
		// 
		Num7 = RootT.Find("kebord/7").gameObject;
		UIEventClick.Set(Num7, OnNum7Click);
		// 
		Num8 = RootT.Find("kebord/8").gameObject;
		UIEventClick.Set(Num8, OnNum8Click);
		// 
		Num9 = RootT.Find("kebord/9").gameObject;
		UIEventClick.Set(Num9, OnNum9Click);
		// 
		Cancel = RootT.Find("kebord/c").gameObject;
		UIEventClick.Set(Cancel, OnCancelClick);
		// 
		Enter = RootT.Find("kebord/enter").gameObject;
		UIEventClick.Set(Enter, OnEnterClick);
        // UI_INIT_END
    }

	public override void OnRefresh(uint nFreshID,  object data) 
	{
		switch(nFreshID)
		{
		case (uint)UIRefreshID.SetLevel:
			m_nCurrentLevel = (uint)data;
			break;
 		}
	}

	public override void OnShow()
	{
		IsPlay = false;

		CreateQuestion();
	}

	bool m_bPlus = false;
	int m_nFirst = 0;
	int m_nSecond = 0;
	int m_nResult = 0;

	int m_nInput = -1;

	private void CreateQuestion()
	{
		int nNum = UnityEngine.Random.Range(0, 10);

		m_nResult = UnityEngine.Random.Range(12, 18);
		m_nFirst = UnityEngine.Random.Range(0, m_nResult+1);
		m_nSecond = m_nResult - m_nFirst;

		if(nNum > 5)
		{
			m_bPlus = true;
			Question.text = $"{m_nFirst} + {m_nSecond} =";
			Answer.text = "";
			m_nInput = -1;
		}
		else
		{
			m_bPlus = false;
			Question.text = $"{m_nResult} - {m_nFirst} =";
			Answer.text = "";
			m_nInput = -1;
		}
	}

	private void SetInput(int nNum)
	{
		if(m_nInput < 0)
		{
			m_nInput = nNum;
		}
		else
		{
			if(m_nInput >= 10)
				return;

			m_nInput = m_nInput * 10 + nNum;
		}

		Answer.text = $"{m_nInput}";
	}

	private void CancelInput()
	{
		string text = "";
		if(m_nInput >= 10)
		{
			m_nInput = m_nInput/10;
			text = m_nInput.ToString();
		}
		else
		{
			m_nInput = -1;
		}

		Answer.text = text;
	}

	// 
	private void OnNum0Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(0);
	}

	// 
	private void OnNum1Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(1);
	}

	// 
	private void OnNum2Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(2);
	}

	// 
	private void OnNum3Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(3);
	}

	// 
	private void OnNum4Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(4);
	}

	// 
	private void OnNum5Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(5);
	}

	// 
	private void OnNum6Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(6);
	}

	// 
	private void OnNum7Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(7);
	}

	// 
	private void OnNum8Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(8);
	}

	// 
	private void OnNum9Click(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		SetInput(9);
	}

	// 
	private void OnCancelClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		CancelInput();
	}

	// 

	bool IsPlay = false;
	private void OnEnterClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		if(IsPlay)
			return;

		IsPlay = true;

		if(m_bPlus)
		{
			if(m_nInput != m_nResult)
			{
				AudioManager.Instance.PlayFXAudio(ClipID.Wrong);
				//CreateQuestion();
				IsPlay = false;
				return;
			}
		}
		else
		{
			if(m_nInput != m_nSecond)
			{
				//CreateQuestion();
				AudioManager.Instance.PlayFXAudio(ClipID.Wrong);
				IsPlay = false;
				return;
			}
		}

		uint nCurrent = Level.Instance.CurrentLifeCount;
        if(nCurrent == 0)
        {
            return;
        }

        nCurrent --;
        Level.Instance.CurrentLifeCount = nCurrent;

        Level.Instance.Current.CurrentLevel = m_nCurrentLevel;
        Level.Instance.Load();
		Close();
	}


	// 
	private void OnBtnCloseClick(GameObject go)
	{
		AudioManager.Instance.PlayUIAudio(ClipID.SngleSelect);
		Close();
	}

    // UI_FUNC_APPEND
}
