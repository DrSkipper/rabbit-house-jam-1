using UnityEngine;

public class Food : MonoBehaviour
{
    public const float ROTATION_SPEED = 2.5f;
    public const float DEATH_DURATION = 0.2f;
    public const float END_SCALE = 1.7f;
    private bool _dying;
    private float _deathTime;

    void Start()
    {
        GlobalEvents.Notifier.SendEvent(new FoodSpawnEvent(this.transform));
    }

    void Update()
    {
        if (!_dying)
        {
            this.transform.Rotate(Vector3.up, ROTATION_SPEED);
        }
        else
        {
            if (_deathTime >= DEATH_DURATION)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _deathTime += Time.deltaTime;
                float s = Easing.SineEaseIn(_deathTime, 1.1f, END_SCALE, DEATH_DURATION);
                this.transform.localScale = new Vector3(s, s, s);
            }
        }
    }

    public void Consume()
    {
        _dying = true;
        GlobalEvents.Notifier.SendEvent(new FoodDestroyedEvent(this.transform));
    }
}
