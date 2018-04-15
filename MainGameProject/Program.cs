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

    // класс "ПЕРСОНАЖ"

    public class Player : IComparable<Player>  // класс персонажа
    {
        // Характеристики персонажа
        protected readonly Guid PlayerID; // ID персонажа
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
            return "Hero name:" + PlayerName.ToString() + ", Race: " + PlayerRace.ToString() + ", Sex: " + PlayerSex.ToString() + ", Age: " + PlayerAge.ToString() + ", Condition: " + PlayerCondition.ToString() + ", Max Health: " + PlayerMaxHealth.ToString() + ", Current Health: " + PlayerCurrentHealth.ToString() + ", Experiance: " + PlayerExp.ToString() + ".";
        }

    }

    // КЛАСС "МАГИЧЕСКИЙ ПЕРСОНАЖ"

    public class MagicPlayer : Player
    {

        public int PlayerCurrentMana { get; set; } // текущее состояние маны
        public int PlayerMaxMana { get; set; } // максимальное кол-во маны

        public void ApplyHeal(int heal,MagicPlayer person)  // пополнение здоровья
        {
            RegenHealth Health = new RegenHealth(heal);
            Health.UseSkill(person, heal);

        }
        public void UseSkillMana(int UseMana)   // использование маны и проверка на ее кол-во
        {
            if (UseMana <= PlayerCurrentMana)
                PlayerCurrentHealth -= UseMana;
            else
                throw new Exception("У вас недостаточно маны!");
        }


        public MagicPlayer(string playerName, string playerRace, string playerSex) : base(playerName, playerRace, playerSex)// конструктор для неизменяемых полей, взятый из класса предка
        { }


    }
    public interface IMagic  // интерфейс волшебство
    {
        void UseSkill(MagicPlayer person = null , int damage = 0);     
    }

    //МАГИЧЕСКИЕ ЗАКЛИНАНИЯ

    public abstract class Skill : IMagic
    {
        public int SkillMinMana { get; set; } // минимальное значение маны
        public int SkillCast { get; set; } // вербальное использование скилла
        public int SkillAction { get; set; } // движение при использовании скилла 

        public abstract void UseSkill(MagicPlayer person = null, int damage = 0);
    }

    // классы заклинаний :

    public class RegenHealth : Skill // Добавление здоровья
    {
      public RegenHealth(int PlusHealthbuff) {
            PlusHealth = PlusHealthbuff;
        }
        public int PlusHealth { get; set; } // минимальное значение маны
        public override void UseSkill (MagicPlayer person = null, int heal = 0)
        {
            heal = PlusHealth;
            SkillMinMana = PlusHealth * 2;
            person.UseSkillMana(SkillMinMana);
            person.PlayerCurrentHealth += PlusHealth;
            if (person.PlayerCurrentHealth > person.PlayerMaxHealth)
            {
                person.PlayerCurrentHealth = person.PlayerMaxHealth;
            }
        }
    }

    public class Heal : Skill // Вылечить
    {
        public override void UseSkill(MagicPlayer person = null, int damage = 0)
        {
            SkillMinMana = 20;
            person.UseSkillMana(SkillMinMana);
            if (person.PlayerCondition == "болен")
                person.PlayerCondition = "здоров";
        }
    }

    public class Antidote : Skill // противоядие
    {
        public override void UseSkill(MagicPlayer person = null, int damage = 0)
        {
            SkillMinMana = 30;
            if (person.PlayerCondition == "отравлен")
            {
                person.UseSkillMana(SkillMinMana);
                person.PlayerCondition = "здоров";
            }
            else
                throw new Exception("Персонаж не отравлен!");
        }
    }

    public class Reincarnation : Skill // возродить/оживить
    {
        public override void UseSkill(MagicPlayer person = null, int damage = 0)
        {
            SkillMinMana = 150;
            if (person.PlayerCondition == "мертв")
            {
                person.UseSkillMana(SkillMinMana);
                person.PlayerCondition = "ослаблен";
                person.PlayerCurrentHealth = 1;
            }
            else
            {
                throw new Exception("Персонаж жив!");
            }
        }
    }

    public class Armor : Skill // броня ( сделать неуязвимым )
    {
        public override void UseSkill(MagicPlayer person = null, int time = 0)
        {
            SkillMinMana = 50 * time;
            person.UseSkillMana(SkillMinMana);
            // тут работа со временем, обработать в UNITY!!!!!
        }
    }

    public class Paralyzed : Skill // отомри
    {
        public override void UseSkill(MagicPlayer person = null, int damage = 0)
        {
            SkillMinMana = 85;
            if (person.PlayerCondition == "парализован")
            {
                person.UseSkillMana(SkillMinMana);
                person.PlayerCondition = "здоров";
                person.PlayerCurrentHealth = 1;
            }
        }
    }


    //АРТЕФАКТЫ


    public abstract class Artefact : IMagic  // абстрактный класс артефактов
    {
        public int ArtefactMana { get;  } // мана артефакта
        public bool ArtefactResume { get; protected set; } // возобновляемый/не возобновляемый 
        protected Artefact(bool Resum, int Mana)
        {
            ArtefactResume = Resum;
            ArtefactMana = Mana;
        }
        public abstract void UseSkill(MagicPlayer person, int damage = 0);

    }


    public class BottleWithHealth : Artefact // бутылка с живой водой 
    {
        int PlusHealth { get; }
        BottleWithHealth(int Resum, int Mana,int PlusHeath) : base(false, Mana)
        {
            PlusHealth = PlusHealth;
        }
        public override void UseSkill(MagicPlayer person = null, int damage = 0)
        {
            damage = PlusHealth;
            person.UseSkillMana(ArtefactMana);
            person.PlayerCurrentHealth += damage;
            if (person.PlayerCurrentHealth > person.PlayerMaxHealth)
                person.PlayerCurrentHealth = person.PlayerMaxHealth;
        }
    }

    public class BottleWithDeath : Artefact // бутылка с мертвой водой водой 
    {
        int PlusMana { get; }
        BottleWithDeath(int Resum, int Mana, int ManaPlus) : base(false, Mana)
        {
            PlusMana = ManaPlus;
        }

        public override void UseSkill(MagicPlayer person = null, int damage = 0)
        {
            damage = PlusMana;
            person.PlayerCurrentMana += damage;
            if (person.PlayerCurrentMana > person.PlayerMaxMana)
                person.PlayerCurrentMana = person.PlayerMaxMana;
        }
    }

    public class lightningStuff : Artefact // посох "Молний"
    {
        public int Damage { get; private set; }
        lightningStuff(int Resum, int Mana, int damage) : base(true, Mana)
        {
            Damage = damage;
        }

        public override void UseSkill(MagicPlayer person, int damage)
        {
            if (Damage != 0)
            {
                person.UseSkillMana(ArtefactMana);
                if (person.PlayerCurrentMana > person.PlayerMaxMana)
                    person.PlayerCurrentMana = person.PlayerMaxMana;
                Damage -= damage;
            }
            else
                ArtefactResume = false;
        }
    }

    public class Item
    {

    }

}
