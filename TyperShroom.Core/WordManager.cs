
namespace TyperShroom.Core;

public class WordManager {
    List<string> easyWords = new List<string>();
    List<string> mediumWords = new List<string>();
    List<string> hardWords = new List<string>();

    private void Shuffle(List<string> list) {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1) {
            n--;
            int randomNum = rng.Next(n + 1);
            string randomWord = list[randomNum];
            list[randomNum] = list[n];
            list[n] = randomWord;
        }
    }

    public WordManager() {
        string[] lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "words.txt"));

        string flag = "";
        foreach (string word in lines) {
            if (string.IsNullOrWhiteSpace(word)) continue;

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

        Shuffle(easyWords);
        Shuffle(mediumWords);
        Shuffle(hardWords);
    }

    public string GetWord(int currentWave, List<char> firstLettersOnScreen, List<Bug>activeBugs) {
        // - Wave 1-2: easy words only
        // - Wave 3-4: easy + medium words mixed
        // - Wave 5-6: medium words only
        // - Wave 7-8: medium + hard words mixed
        // - Wave 9+: hard words only

        List<string> activeWords = new List<string>();
        foreach(Bug bug in activeBugs) {
            activeWords.Add(bug.Word);
        }

        // waves (1 - 2)
        if (currentWave <= 2) {
            foreach (string word in easyWords) {
                if (!firstLettersOnScreen.Contains(word[0]) && !activeWords.Contains(word)) {
                    easyWords.Remove(word);
                    return word;
                }
            }
        }
        // waves (3 - 4)
        else if (currentWave >= 3 && currentWave <= 4) {
            // make it 50-50 chance for choosing word difficulty
            Random rng = new Random();
            List<string>[] lists;
            if (rng.Next(2) == 0)
                lists = new[] { easyWords, mediumWords };
            else
                lists = new[] { mediumWords, easyWords };

            
            foreach (List<string> list in lists) {
                foreach (string word in list) {
                    if (!firstLettersOnScreen.Contains(word[0]) && !activeWords.Contains(word)){
                        if (easyWords.Contains(word)) easyWords.Remove(word);
                        else if (mediumWords.Contains(word)) mediumWords.Remove(word);
                        return word;
                    }
                }
            }
        }
        // waves (5 - 6)
        else if (currentWave >= 5 && currentWave <= 6) {
            foreach (string word in mediumWords) {
                if (!firstLettersOnScreen.Contains(word[0]) && !activeWords.Contains(word)){
                    mediumWords.Remove(word);
                    return word;
                }
            }
        }
        // waves (7 - 8)
        else if (currentWave >= 7 && currentWave <= 8) {
            // make it 50-50 chance for choosing word difficulty
            Random rng = new Random();
            List<string>[] lists;
            if (rng.Next(2) == 0)
                lists = new[] { hardWords, mediumWords };
            else
                lists = new[] { mediumWords, hardWords };

            foreach (List<string> list in lists) {
                foreach (string word in list) {
                    if (!firstLettersOnScreen.Contains(word[0]) && !activeWords.Contains(word)){
                        if (hardWords.Contains(word)) hardWords.Remove(word);
                        else if (mediumWords.Contains(word)) mediumWords.Remove(word);
                        return word;
                    }
                }
            }
        }
        // waves (9+)
        else if (currentWave >= 9) {
            foreach (string word in hardWords) {
                if (!firstLettersOnScreen.Contains(word[0]) && !activeWords.Contains(word)) {
                    hardWords.Remove(word);
                    return word;
                }
            }
        }

        return "";
    }
}