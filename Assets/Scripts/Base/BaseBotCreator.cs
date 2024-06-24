using UnityEngine;

public class BaseBotCreator : MonoBehaviour
{
    [SerializeField] private Transform _botParrent;
    [SerializeField] private Bot _botPrefab;

    public Transform BotParrent { get => _botParrent; }

    public Bot Create()
    {
        Bot bot = Instantiate(_botPrefab);
        bot.transform.SetParent(_botParrent);

        return bot;
    }
}