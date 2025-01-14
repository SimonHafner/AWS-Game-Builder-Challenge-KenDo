using System.Collections.Generic;

public class InterfaceText
{
    private static InterfaceText instance;

    public static InterfaceText Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new InterfaceText();
            }
            return instance;
        }
    }

    // Hardcodierte Übersetzungsdaten
    private Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>()
{
   { "de", new Dictionary<string, string>() {
       { "Level", "Level:" },
       { "Mistakes", "Fehler:" },
       { "ClickInstruction", "Tippe auf eine beliebige Stelle, um fortzufahren!" },
       { "Solved", "Gelöst!" },
       { "Continue", "Weiter" },
       { "Play", "Spielen" },
       { "Options", "Optionen" }
   }},
   { "en", new Dictionary<string, string>() {
       { "Level", "Level:" },
       { "Mistakes", "Mistakes:" },
       { "ClickInstruction", "Tap anywhere to continue!" },
       { "Solved", "Solved!" },
       { "Continue", "Continue" },
       { "Play", "Play" },
       { "Options", "Options" }
   }},
   { "fr", new Dictionary<string, string>() {
       { "Level", "Niveau:" },
       { "Mistakes", "Erreurs:" },
       { "ClickInstruction", "Appuyez n'importe où pour continuer!" },
       { "Solved", "Résolu!" },
       { "Continue", "Continuer" },
       { "Play", "Jouer" },
       { "Options", "Options" }
   }},
   { "it", new Dictionary<string, string>() {
       { "Level", "Livello:" },
       { "Mistakes", "Errori:" },
       { "ClickInstruction", "Tocca un punto qualsiasi per continuare!" },
       { "Solved", "Risolto!" },
       { "Continue", "Continua" },
       { "Play", "Gioca" },
       { "Options", "Opzioni" }
   }},
   { "es", new Dictionary<string, string>() {
       { "Level", "Nivel:" },
       { "Mistakes", "Errores:" },
       { "ClickInstruction", "¡Toca en cualquier lugar para continuar!" },
       { "Solved", "¡Resuelto!" },
       { "Continue", "Continuar" },
       { "Play", "Jugar" },
       { "Options", "Opciones" }
   }},
   { "pt", new Dictionary<string, string>() {
       { "Level", "Nível:" },
       { "Mistakes", "Erros:" },
       { "ClickInstruction", "Toque em qualquer lugar para continuar!" },
       { "Solved", "Resolvido!" },
       { "Continue", "Continuar" },
       { "Play", "Jogar" },
       { "Options", "Opções" }
   }},
   { "ru", new Dictionary<string, string>() {
       { "Level", "Уровень:" },
       { "Mistakes", "Ошибки:" },
       { "ClickInstruction", "Нажмите в любом месте, чтобы продолжить!" },
       { "Solved", "Решено!" },
       { "Continue", "Продолжить" },
       { "Play", "Играть" },
       { "Options", "Настройки" }
   }},
   { "zh", new Dictionary<string, string>() {
       { "Level", "等级:" },
       { "Mistakes", "错误:" },
       { "ClickInstruction", "点击任意位置继续！" },
       { "Solved", "已解决！" },
       { "Continue", "继续" },
       { "Play", "开始" },
       { "Options", "选项" }
   }},
   { "ja", new Dictionary<string, string>() {
       { "Level", "レベル:" },
       { "Mistakes", "間違い:" },
       { "ClickInstruction", "どこでもタップして続行！" },
       { "Solved", "解決！" },
       { "Continue", "続ける" },
       { "Play", "プレイ" },
       { "Options", "設定" }
   }}
};

    // Privater Konstruktor, um die Instanziierung von außen zu verhindern
    private InterfaceText() { }

    public string GetText(string languageCode, string key)
    {
        if (translations.ContainsKey(languageCode))
        {
            var langDict = translations[languageCode];
            if (langDict.ContainsKey(key))
            {
                return langDict[key];
            }
        }
        return key; // Wenn keine Übersetzung gefunden wurde, geben wir den Schlüssel zurück
    }
}
