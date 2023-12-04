using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabMenuFunctions : MonoBehaviour
{
    public Player player;

    public void UpgradeSwordButton()
    {
        player.UpgradeSkill(0);
    }

    public void UpgradeBowButton()
    {
        player.UpgradeSkill(1);
    }

    public void UpgradeMagicButton()
    {
        player.UpgradeSkill(2);
    }
}
