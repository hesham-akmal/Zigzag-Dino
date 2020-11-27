using UnityEngine;
using SaveSystem;

public class BankManager : MonoBehaviour
{
    #region SingletonAndAwake

    private static BankManager _instance;
    public static BankManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion SingletonAndAwake

    private Cryptography crypto;

    private void Start()
    {
        crypto = new Cryptography();
    }

    private void CheckFirstInit()
    {
        //If no key, then save 0 coins in bank.
        if (!EasySave.HasKey<string>("BankCoins"))
            EasySave.Save("BankCoins", crypto.Encrypt(0));
    }

    public int getBankCoins()
    {
        CheckFirstInit();
        string encryptedBankCoins = EasySave.Load<string>("BankCoins");
        return crypto.Decrypt<int>(encryptedBankCoins);
    }

    public void AddCoins(int Coins)
    {
        int BankCoins = getBankCoins();
        BankCoins += Coins;

        EasySave.Save("BankCoins", crypto.Encrypt(BankCoins));

        ((DeathSG)UIManager.Instance.DeathSG).UpdateBankTMP();
    }

    /// <param name="Coins"></param>
    /// <returns>True if successful deduction</returns>
    public bool DeductCoins(int Coins)
    {
        int BankCoins = getBankCoins();

        if (BankCoins >= Coins)
        {
            BankCoins -= Coins;

            EasySave.Save("BankCoins", crypto.Encrypt(BankCoins));
            ((DeathSG)UIManager.Instance.DeathSG).UpdateBankTMP();

            return true;
        }
        return false;
    }
}