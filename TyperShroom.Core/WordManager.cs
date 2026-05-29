
namespace TyperShroom.Core;

public class WordManager {
    List<string> easyWords = new List<string>();
    List<string> mediumWords = new List<string>();
    List<string> hardWords = new List<string>();

    public WordManager() {
        string[] lines = File.ReadAllLines("words.txt");

        string flag = "";
        foreach (string word in lines) {
            if (word == "[easy]") {
                flag = "easy";
                continue;
            } 
            if (word == "[medium]") {
                flag = "medium";
                continue;
            }
            if (word == "[hard]") {
                flag = "hard";
                continue;
            }

            if (flag == "easy") easyWords.Add(word);
            if (flag == "medium") mediumWords.Add(word);
            if (flag == "hard") hardWords.Add(word);
        }
    }


    public string GetWord(int currentWave, List<char> FirstLettersOnScreen) {
        // easy words waves (1 - 2)
        if (currentWave <= 2) {
            foreach (string word in easyWords) {
                if (!FirstLettersOnScreen.Contains(word[0]))
                    return word;
            }
        }
        // medium words waves (3 - 5)
        else if (currentWave >= 3 && currentWave <= 5) {
            foreach (string word in mediumWords) {
                if (!FirstLettersOnScreen.Contains(word[0]))
                    return word;
            }
        }
        // hard words waves (6+)
        else if (currentWave >= 6) {
            foreach (string word in hardWords) {
                if (!FirstLettersOnScreen.Contains(word[0]))
                    return word;
            }
        }

        return "";
    }
}