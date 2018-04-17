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
            MagicPlayer Max = new MagicPlayer("Maxim", Player.PlayerRace.Человек, Player.PlayerSex.Мужчина);
            Max.PlayerCurrentHealth = 100;
            Max.PlayerCurrentMana = 300;
            Max.PlayerMaxMana = 300;
            Max.PlayerMaxHealth = 100;
            Max.PlayerExp = 1000;
            Max.PlayerAge = 18;
            Max.Condition = Player.PlayerCondition.Отравлен;
            Paralyzed paralyzed = new Paralyzed();
            FrogLegs legs = new FrogLegs();
            Max.LearnSkill(paralyzed);
            Max.AddToBackpack(legs,1);
            try
            {
                paralyzed.UseSkill(Max);
                Max.UseArtefact(legs, Max);
            }
            catch (InvalidCastException e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine(Max.ToString());
            }
    }

    // класс "ПЕРСОНАЖ"

    public class Player : IComparable<Player>  // класс персонажа
    {
        // Характеристики персонажа
        protected readonly Guid PlayerID; // ID персонажа
        protected readonly string PlayerName; // Имя персонажа
        public PlayerCondition Condition; // состояние персонажа
        protected readonly  PlayerRace Race; //раса персонажа
        protected readonly  PlayerSex Sex; //пол
        public int PlayerAge { get; set; } // возраст
        public int PlayerCurrentHealth { get; set; } // текущее состояние здоровья
        public int PlayerMaxHealth { get; set; } // максимальное здоровье
        public int PlayerExp { get; set; }// кол-во набранного опыта

        public enum PlayerCondition { Здоров, Мертв, Парализован, Отравлен, Ослаблен,Болен};
        public enum PlayerRace { Человек, Эльф, Орк, Нежить, Гном };
        public enum PlayerSex { Мужчина, Женщина };


        // инвентарь
        public Dictionary<Artefact, int> Backpack;
        public int BackpackSize = 6;

        public Player(string playerName, PlayerRace playerRace, PlayerSex playerSex) // конструктор для неизменяемых полей
        {
            PlayerID = Guid.NewGuid();
            PlayerName = playerName;
            Race = playerRace;
            Sex = playerSex;
            Backpack = new Dictionary<Artefact, int>();
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
                Condition = PlayerCondition.Мертв;
            if (PlayerCurrentHealth < 10 && PlayerCurrentHealth > 0)
                Condition = PlayerCondition.Ослаблен;
            if (PlayerCurrentHealth >= 10)
                Condition = PlayerCondition.Здоров;
            if (PlayerCurrentHealth > PlayerMaxHealth)
                PlayerCurrentHealth = PlayerMaxHealth;
        }
        public override string ToString()  //вывод информации о герое
        {
            return "Hero name:" + PlayerName.ToString() + ", Race: " + Race.ToString() + ", Sex: " + Sex.ToString() + ", Age: " + PlayerAge.ToString() + ", Condition: " + Condition.ToString() + ", Max Health: " + PlayerMaxHealth.ToString() + ", Current Health: " + PlayerCurrentHealth.ToString() + ", Experiance: " + PlayerExp.ToString();
        }


        public bool AddToBackpack (Artefact icon, int count) // добавить артефакт в инвентарь
        {
            if (Backpack.ContainsKey(icon))
            {
                Backpack[icon] += count;
                return true;
            }
            else
            {
                if (Backpack.Count >= BackpackSize)
                    return false;

                Backpack.Add(icon, count);
                return true;
            }

        }

        public bool DeleteFromBackpakc(Artefact icon, int count)  // выбросить артефакт
        {
            if (!Backpack.ContainsKey(icon))
                return false;

            if (Backpack[icon] == 1)
                Backpack.Remove(icon);
            else
                Backpack[icon] -= count;
            return true;
        }



        public bool UseArtefact (Artefact icon, MagicPlayer person = null, Player enemy = null, int power = 0) // Использовать артефакт из инвенторя
        {
            if (!this.Backpack.ContainsKey(icon))
                return false;
            icon.UseSkill(person,power = 0,enemy = null);
            return true;   
        }

        public bool GiveArtefact(Artefact icon, MagicPlayer person, int count = 1)  // отдать артефакт другому персонажу
        {
            if (!this.Backpack.ContainsKey(icon) || person.Backpack.Count() == person.BackpackSize)
                return false;

            this.DeleteFromBackpakc(icon, count);
            person.AddToBackpack(icon, count);

            return true;
        }

    }

    // КЛАСС "МАГИЧЕСКИЙ ПЕРСОНАЖ"

    public class MagicPlayer : Player
    {

        public int PlayerCurrentMana { get; set; } // текущее состояние маны
        public int PlayerMaxMana { get; set; } // максимальное кол-во маны
        public List<Skill> Skills; // Список скиллов

        public void ApplyHeal(int heal,MagicPlayer person)  // пополнение здоровья
        {
            RegenHealth Health = new RegenHealth(heal);
            Health.UseSkill(person, heal);

        }
        public void UseSkillMana(int UseMana)   // использование маны и проверка на ее кол-во
        {
            if (UseMana <= PlayerCurrentMana)
                PlayerCurrentMana -= UseMana;
            else
                throw new InvalidCastException("У вас недостаточно маны!");
        }


        public MagicPlayer(string playerName, PlayerRace playerRace, PlayerSex playerSex) : base(playerName, playerRace, playerSex)// конструктор для неизменяемых полей, взятый из класса предка
        {
            Skills = new List<Skill>();
        }

        public void LearnSkill(Skill skill)  // выучить скилл
        {
            Skills.Add(skill);
        }

        public void ForgetSkill(Skill skill)
        {
            Skills.Remove(skill);
        }

        public override string ToString()
        {
            return base.ToString() + ", Mana points: " + PlayerCurrentMana + ", Max mana: " + PlayerMaxMana;
        }


    }
    public interface IMagic  // интерфейс волшебство
    {
        void UseSkill(MagicPlayer person = null , int damage = 0, Player enemy = null);     
    }

    //МАГИЧЕСКИЕ ЗАКЛИНАНИЯ

    public abstract class Skill : IMagic
    {
        public int SkillMinMana { get; set; } // минимальное значение маны
        public int SkillCast { get; set; } // вербальное использование скилла
        public int SkillAction { get; set; } // движение при использовании скилла 

        public abstract void UseSkill(MagicPlayer person = null, int damage = 0, Player enemy = null);
    }

    // классы заклинаний :

    public class RegenHealth : Skill // Добавление здоровья
    {
      public RegenHealth(int PlusHealthbuff) {
            PlusHealth = PlusHealthbuff;
        }
        public int PlusHealth { get; set; } // минимальное значение маны
        public override void UseSkill (MagicPlayer person , int heal = 0, Player enemy = null)
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
        public override void UseSkill(MagicPlayer person , int damage = 0, Player enemy = null)
        {
            SkillMinMana = 20;
            person.UseSkillMana(SkillMinMana);
            if (person.Condition == Player.PlayerCondition.Болен)
                person.Condition = Player.PlayerCondition.Здоров;
        }
    }

    public class Antidote : Skill // противоядие
    {
        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
        {
            SkillMinMana = 30;
            if (person.Condition == Player.PlayerCondition.Отравлен)
            {
                person.UseSkillMana(SkillMinMana);
                person.Condition = Player.PlayerCondition.Здоров;
            }
            else
                throw new Exception("Персонаж не отравлен!");
        }
    }

    public class Reincarnation : Skill // возродить/оживить
    {
        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
        {
            SkillMinMana = 150;
            if (person.Condition == Player.PlayerCondition.Мертв)
            {
                person.UseSkillMana(SkillMinMana);
                person.Condition = Player.PlayerCondition.Ослаблен;
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
        public override void UseSkill(MagicPlayer person, int time = 0, Player enemy = null)
        {
            SkillMinMana = 50 * time;
            person.UseSkillMana(SkillMinMana);
            // тут работа со временем, обработать в UNITY!!!!!
        }
    }

    public class Paralyzed : Skill // отомри
    {
        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
        {
            SkillMinMana = 85;
            if (person.Condition == Player.PlayerCondition.Парализован)
            {
                person.UseSkillMana(SkillMinMana);
                person.Condition = Player.PlayerCondition.Здоров;
                person.PlayerCurrentHealth = 1;
            }
        }
    }


    //АРТЕФАКТЫ


    public abstract class Artefact : IMagic  // абстрактный класс артефактов
    {
        public int ArtefactMana { get;  } // мана артефакта
        public bool ArtefactResume { get; protected set; } // возобновляемый/не возобновляемый 
        string ArtefactName { get; } //название предмета
        protected Artefact(bool Resum, int Mana,string name)
        {
            ArtefactResume = Resum;
            ArtefactMana = Mana;
            ArtefactName = name;
        }
        public abstract void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null);

    }


    public class BottleWithHealth : Artefact // бутылка с живой водой 
    {
        int PlusHealth { get; }
        public BottleWithHealth(int PlusHeath) : base(false, 50," бутылка с живой водой")
        {
            PlusHealth = PlusHealth;
        }
        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
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
        public BottleWithDeath( int ManaPlus) : base(false, 0, "бутылка с мертвой водой водой")
        {
            PlusMana = ManaPlus;
        }

        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
        {
            damage = PlusMana;
            person.PlayerCurrentMana += damage;
            if (person.PlayerCurrentMana > person.PlayerMaxMana)
                person.PlayerCurrentMana = person.PlayerMaxMana;
        }
    }

    public class LightningStuff : Artefact // посох "Молний"
    {
        public int Damage { get; private set; }
        public LightningStuff( int damage) : base(true, 50, "посох Молний")
        {
            Damage = damage;
        }

        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
        {
            if (Damage != 0)
            {
                person.UseSkillMana(ArtefactMana);
                enemy.ApplyDamage(damage);
                Damage -= damage;
            }
            else
                ArtefactResume = false;
        }
    }

    public class FrogLegs : Artefact // лягушачьи лапки
    {
        public FrogLegs() : base(false, 50, "лягушачьи лапки") { }
        public override void UseSkill(MagicPlayer person, int damage = 0, Player enemy = null)
        {
            if (person.Condition == Player.PlayerCondition.Отравлен)
            {
                person.UseSkillMana(ArtefactMana);
                person.Condition = Player.PlayerCondition.Здоров;
            }

        }

    }

    public class PoisonTouch : Artefact // ядовитая слюна
    {
        public int Damage { get; private set; }
        public PoisonTouch (int damage) : base (true, 50,"ядовитая слюна")
        {
            Damage = damage;
        }
        public override void UseSkill(MagicPlayer person, int damage , Player enemy )
        {
            if (person.Condition == Player.PlayerCondition.Здоров || person.Condition == Player.PlayerCondition.Ослаблен)
            {
                person.UseSkillMana(ArtefactMana);
                enemy.ApplyDamage(Damage);
                if (enemy.Condition != Player.PlayerCondition.Мертв)
                    enemy.Condition = Player.PlayerCondition.Отравлен;

            }

        }
    }

    public class BasiliskEye : Artefact // глаз Василиска
    {
        BasiliskEye() : base(false, 50, "глаз Василиска") { }
        public override void UseSkill(MagicPlayer person, int damage, Player enemy)
        {
            if (enemy.Condition != Player.PlayerCondition.Мертв)
            {
                person.UseSkillMana(ArtefactMana);
                enemy.Condition = Player.PlayerCondition.Парализован;
            }

        }

    }


}
