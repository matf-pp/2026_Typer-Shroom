
namespace TyperShroom.Core;

public class WordManager {
    List<string> easyWords = new List<string>();
    List<string> mediumWords = new List<string>();
    List<string> hardWords = new List<string>();
    int _easyIndex = 0;
    int _mediumIndex = 0;
    int _hardIndex = 0;

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
            for (int i = _easyIndex; i < easyWords.Count; i++) {
                if (!firstLettersOnScreen.Contains(easyWords[i][0]) && !activeWords.Contains(easyWords[i])) {
                    _easyIndex = i + 1;
                    if (_easyIndex >= easyWords.Count) {
                        Shuffle(easyWords);
                        _easyIndex = 0;
                    }
                    return easyWords[i];
                }
            }
        }
        // waves (3 - 4)
        else if (currentWave >= 3 && currentWave <= 4) {
            Random rng = new Random();
            bool easyFirst = rng.Next(2) == 0;
            List<string> primary = easyFirst ? easyWords : mediumWords;
            List<string> secondary = easyFirst ? mediumWords : easyWords;
            ref int primaryIndex = ref (easyFirst ? ref _easyIndex : ref _mediumIndex);
            ref int secondaryIndex = ref (easyFirst ? ref _mediumIndex : ref _easyIndex);

            for (int i = primaryIndex; i < primary.Count; i++) {
                if (!firstLettersOnScreen.Contains(primary[i][0]) && !activeWords.Contains(primary[i])) {
                    primaryIndex = i + 1;
                    if (primaryIndex >= primary.Count) { Shuffle(primary); primaryIndex = 0; }
                    return primary[i];
                }
            }
            for (int i = secondaryIndex; i < secondary.Count; i++) {
                if (!firstLettersOnScreen.Contains(secondary[i][0]) && !activeWords.Contains(secondary[i])) {
                    secondaryIndex = i + 1;
                    if (secondaryIndex >= secondary.Count) { Shuffle(secondary); secondaryIndex = 0; }
                    return secondary[i];
                }
            }
        }
        // waves (5 - 6)
        else if (currentWave >= 5 && currentWave <= 6) {
            for (int i = _mediumIndex; i < mediumWords.Count; i++) {
                if (!firstLettersOnScreen.Contains(mediumWords[i][0]) && !activeWords.Contains(mediumWords[i])) {
                    _mediumIndex = i + 1;
                    if (_mediumIndex >= mediumWords.Count) { Shuffle(mediumWords); _mediumIndex = 0; }
                    return mediumWords[i];
                }
            }
        }
        // waves (7 - 8)
        else if (currentWave >= 7 && currentWave <= 8) {
            Random rng = new Random();
            bool hardFirst = rng.Next(2) == 0;
            List<string> primary = hardFirst ? hardWords : mediumWords;
            List<string> secondary = hardFirst ? mediumWords : hardWords;
            ref int primaryIndex = ref (hardFirst ? ref _hardIndex : ref _mediumIndex);
            ref int secondaryIndex = ref (hardFirst ? ref _mediumIndex : ref _hardIndex);

            for (int i = primaryIndex; i < primary.Count; i++) {
                if (!firstLettersOnScreen.Contains(primary[i][0]) && !activeWords.Contains(primary[i])) {
                    primaryIndex = i + 1;
                    if (primaryIndex >= primary.Count) { Shuffle(primary); primaryIndex = 0; }
                    return primary[i];
                }
            }
            for (int i = secondaryIndex; i < secondary.Count; i++) {
                if (!firstLettersOnScreen.Contains(secondary[i][0]) && !activeWords.Contains(secondary[i])) {
                    secondaryIndex = i + 1;
                    if (secondaryIndex >= secondary.Count) { Shuffle(secondary); secondaryIndex = 0; }
                    return secondary[i];
                }
            }
        }
        // waves (9+)
        else if (currentWave >= 9) {
            for (int i = _hardIndex; i < hardWords.Count; i++) {
                if (!firstLettersOnScreen.Contains(hardWords[i][0]) && !activeWords.Contains(hardWords[i])) {
                    _hardIndex = i + 1;
                    if (_hardIndex >= hardWords.Count) { Shuffle(hardWords); _hardIndex = 0; }
                    return hardWords[i];
                }
            }
        }

        return "";
    }
}