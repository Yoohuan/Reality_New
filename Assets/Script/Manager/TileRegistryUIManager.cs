using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TileRegistryUIManager : MonoBehaviour
{
    public GameObject scrollViewContent; // Scroll View包裹的范围
    public GameObject tileButtonPrefab;  // 按钮


    private TileBase[] registries; // 地块注册表

    public static TileRegistryUIManager Instance;

    public LevelManager levelManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        registries = LevelManager.Instance.registries;

        float currentButtonY = 0f; // 用于追踪按钮的当前垂直位置

        for (int i = 0; i < registries.Length; i++)
        {
            GameObject tileButton = Instantiate(tileButtonPrefab, scrollViewContent.transform);

            // 手动挪位置，不过这样似乎在滚轮方面有问题
            RectTransform buttonRectTransform = tileButton.GetComponent<RectTransform>();
            buttonRectTransform.localPosition = new Vector3(80 + 140 * i, 0, 0);


            //buttonRectTransform.anchoredPosition = new Vector2(buttonRectTransform.anchoredPosition.x, 80 - currentButtonY);

            // 获取按钮上的Text组件，改名
            Text buttonText = tileButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = registries[i].tileName;
            }

           

            // 获取按钮上的TileOnGUI组件
            TileOnGUI tileOnGUIComponent = tileButton.GetComponent<TileOnGUI>();
            if (tileOnGUIComponent)
            {
                tileOnGUIComponent.SetId(i);
                
                Image img = tileButton.GetComponentInChildren<Image>();
                img.sprite = tileOnGUIComponent.GetTile().imgOnGui;

                Button button = tileButton.GetComponentInChildren<Button>();
                if (button)
                {
                    button.onClick.AddListener(() =>
                    {
                        //UIManager.Instance.OnPutClick();
                        UIManager.Instance.OnObstacleClick(tileOnGUIComponent.GetTile());
                    });
                }
            }

            // 按钮间的间隔
            currentButtonY += buttonRectTransform.rect.height + 35f;

        }
        tileButtonPrefab.SetActive(false);
    }



}
