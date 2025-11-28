using Managers;
using Scriptables.Events;
using UnityEngine;

namespace User_Interface
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;
        [SerializeField] private GameObject saveMenu;
        [SerializeField] private GameObject creditsMenu;

        [SerializeField] private EventDatum[] continueEventData;

        public void OpenMenu(int menuIndex)
        {
            mainMenu.SetActive(false);
            saveMenu.SetActive(false);
            creditsMenu.SetActive(false);
            
            switch (menuIndex)
            {
                case 0:
                    mainMenu.SetActive(true);
                    break;
                case 1:
                    saveMenu.SetActive(true);
                    break;
                case 2:

                    break;
                case 3:
                    creditsMenu.SetActive(true);
                    break;
            }
        }

        public void Quit()
        {
            Application.Quit();
        }

        public void StartNewGame()
        {
            EventManager.SendEvents(continueEventData);
            OpenMenu(-1);
        }

        public void LoadSave(int saveId)
        {
            
        }
    }
}
