using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private TriggerZone _triggerZone;
    [SerializeField] private List<Bot> _freeBots;
    [SerializeField] private List<Bot> _busyBots;

    private BaseScanner _baseScanner;
    private BaseResourceStorage _resourceStorage;

    private void Awake()
    {
        _baseScanner = GetComponent<BaseScanner>();
        _resourceStorage = GetComponent<BaseResourceStorage>();

        foreach (var bot in _freeBots)
            bot.Init(this);
    }

    private void OnEnable()
    {
        _triggerZone.BotIsBack += AddToFreeBots;
    }

    private void OnDisable()
    {
        _triggerZone.BotIsBack -= AddToFreeBots;
    }

    private void Update()
    {
        if (_freeBots.Count <= 0)
            return;

        print(_freeBots.Count);
        SendBotToPicking();
    }

    private void AddToFreeBots(Bot bot)
    {
        if (bot != null && _freeBots.Contains(bot) == false)
        {
            _freeBots.Add(bot);
            _busyBots.Remove(bot);
        }
    }

    private void SendBotToPicking()
    {
        _freeBots[0].SetTargetResource(_baseScanner.GetRandomResource());
        _busyBots.Add(_freeBots[0]);
        _freeBots.RemoveAt(0);
    }
}