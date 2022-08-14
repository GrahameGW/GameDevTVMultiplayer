using UnityEngine;
using UnityEngine.UI;


namespace RTSTutorialGame
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] GameObject healthBarObject;
        [SerializeField] Image healthBarImage;


        private void OnEnable()
        {
            health.ClientOnHealthUpdated += HandleHealthUpdated;   
        }

        private void OnDisable()
        {
            health.ClientOnHealthUpdated -= HandleHealthUpdated;
        }

        private void HandleHealthUpdated(int currentHealth, int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth / maxHealth;
        }

        private void OnMouseEnter()
        {
            healthBarObject.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarObject.SetActive(false);
        }
    }
}

