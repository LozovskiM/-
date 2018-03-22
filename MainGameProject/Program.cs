// Ролевая игра (1 курс, 2 семестр ) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMain
{
    class Program
    {
        static void Main(string[] args)
        {
            Player Max = new Player("Maxim", "human", "man");
            Max.PlayerCurrentHealth = 20;
            Max.PlayerMaxHealth = 100;
            Max.PlayerExp = 1000;
            Max.PlayerAge = 17;
            Max.PlayerCondition = "Здоров";
            Console.WriteLine(Max.ToString());
        }
    }
    public class Player : IComparable<Player>  // класс персонажа
    {
        // Характеристики персонажа
        protected readonly Guid PlayerID;
        protected readonly string PlayerName; // Имя персонажа
        public string PlayerCondition; // состояние персонажа
        protected readonly string PlayerRace; //раса персонажа
        protected readonly string PlayerSex; //пол
        public int PlayerAge { get; set; } // возраст
        public int PlayerCurrentHealth { get; set; } // текущее состояние здоровья
        public int PlayerMaxHealth { get; set; } // максимальное здоровье
        public int PlayerExp { get; set; }// кол-во набранного опыта

        public Player(string playerName, string playerRace, string playerSex) // конструктор для неизменяемых полей
        {
            PlayerID = Guid.NewGuid();
            PlayerName = playerName;
            PlayerRace = playerRace;
            PlayerSex = playerSex;
        }

        public int CompareTo(Player obj) // сравнение персонажей по опыту
        {
            if (this.PlayerExp > obj.PlayerExp)
                return 1;
            if (this.PlayerExp < obj.PlayerExp)
                return -1;
            else
                return 0;
        }
        public void ApplyDamage(int damage) // получение урона
        {
            PlayerCurrentHealth -= damage;
            if (PlayerCurrentHealth < 0)
                PlayerCurrentHealth = 0;
            if (PlayerCurrentHealth == 0)
                PlayerCondition = "Мертв";
            if (PlayerCurrentHealth < 10 && PlayerCurrentHealth > 0)
                PlayerCondition = "Ослаблен";
            if (PlayerCurrentHealth >= 10)
                PlayerCondition = "Здоров";
            if (PlayerCurrentHealth > PlayerMaxHealth)
                PlayerCurrentHealth = PlayerMaxHealth;
        }
        public override string ToString()  //вывод информации о герое
        {
            return "Hero name:" + PlayerName.ToString() + ": Race: " + PlayerRace.ToString() + ", Sex: " + PlayerSex.ToString() + ", Age: " + PlayerAge.ToString() + ", Condition: " + PlayerCondition.ToString() + ", Max Health: " + PlayerMaxHealth.ToString() + ", Current Health: " + PlayerCurrentHealth.ToString() + ", Experiance: " + PlayerExp.ToString() + ".";
        }

    }

    public class MagicPlayer : Player
    {

        public int PlayerCurrentMana { get; set; } // текущее состояние маны
        public int PlayerMaxMana { get; set; } // максимальное кол-во маны

        public void ApplyHeal(int heal)  // пополнение здоровья
        {
            if (PlayerCurrentMana / 2 >= heal)
                PlayerCurrentHealth += heal;
            if (PlayerCurrentHealth > PlayerMaxHealth)
            {
                PlayerCurrentHealth = PlayerMaxHealth;
            }

        }
        public void UseSkill(int UseMana)   // использование маны и проверка на ее кол-во
        {
            if (UseMana <= PlayerCurrentMana)
                PlayerCurrentHealth -= UseMana;
        }


        public MagicPlayer(string playerName, string playerRace, string playerSex) : base(playerName, playerRace, playerSex)// конструктор для неизменяемых полей, взятый из класса предка
        { }


    }
    public class Item
    {

    }

    //Тоха пидр

}
