using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace MLITA {
    class MLITA_LAB2 {
        static void Main(string[] args) {
            Console.WriteLine("Enter list of numbers of unit sets of a Boolean function:");
            
            var numbersString = String.Empty;
            while (numbersString == string.Empty) {
                numbersString = Console.ReadLine();
            }

            FindMinDNF(MakeNumbersListFromString(numbersString));
        }

        static List<int> MakeNumbersListFromString(string numbersString) {
            var regexForNumbers = new Regex("\\d+");
            var numbersList = regexForNumbers.Matches(numbersString).Cast<Match>().Select(matchEl => int.Parse(matchEl.Value)).ToList();

            return numbersList;
        }

        static List<String> ConvertNumbersFromDecToBin(List<int> decNumbersList, out uint bitsPerBinNum) {
            var logOfMaxDecNum = Math.Log2(decNumbersList.Max());
            var numberOfBits = (uint) (Math.Ceiling(logOfMaxDecNum));

            if (numberOfBits == logOfMaxDecNum) {
                numberOfBits++;
            }
            var binaryNumbersList = decNumbersList.Select(decNum => {
                var binaryNumStr = Convert.ToString(decNum, 2);
                if (binaryNumStr.Length < numberOfBits) {
                    binaryNumStr = new String('0', (int)numberOfBits-binaryNumStr.Length) + binaryNumStr;
                } 
                return binaryNumStr;
                }).ToList();

            bitsPerBinNum = numberOfBits;
            return binaryNumbersList;
        }

        struct MarkableElement<T> {
            public MarkableElement(T element, bool marked = false) {
                this.element = element;
                this.marked = marked;
            }

            public bool marked;
            public T element;
        };

        static void FindMinDNF(List<int> decConstituentsOfOnes) {
            var binConstituentsOfOnes = ConvertNumbersFromDecToBin(decConstituentsOfOnes, out uint bitsPerBinNum);
            OutputListInConsole(binConstituentsOfOnes);

            Dictionary<uint, List<MarkableElement<string>>> constituentsGroups = new();
            List<uint> constituentsGroupsKeys = new();
            // Подсчитываем количество единиц и разделяем конституенты на группы по количеству единиц
            foreach(var constituent in binConstituentsOfOnes) {
                var numberOfOnesInConstituent = CountSymbolsInString(constituent, '1');
                if (!constituentsGroups.ContainsKey(numberOfOnesInConstituent)) {
                    List<MarkableElement<string>> constituentsGroup = new();
                    constituentsGroups.Add(numberOfOnesInConstituent, constituentsGroup);
                    constituentsGroupsKeys.Add(numberOfOnesInConstituent);
                }
                constituentsGroups[numberOfOnesInConstituent].Add(new MarkableElement<string>(constituent));
            }
            
            if (constituentsGroups.Count > 1) {
                List<string> unusedImplicants = new();
                // пока имеются склеивающиеся импликанты
                Dictionary<string, List<MarkableElement<string>>> implicantsGroups = new();
                int currentConstituentsGroupKeyId = 0;
                uint firstConstituentsGroupKey = constituentsGroupsKeys[currentConstituentsGroupKeyId++];
                uint secondConstituentsGroupKey = constituentsGroupsKeys[currentConstituentsGroupKeyId++];
                while(true) {
                    foreach(var firstGroupConstituent in constituentsGroups[firstConstituentsGroupKey]) {
                        foreach(var secondGroupConstituent in constituentsGroups[secondConstituentsGroupKey]) {
                            int countOfDifferentBits = 0;
                            string implicantsGroupKey = string.Empty;
                            bool isConstituentsMergeable = false;
                            for (int bitId = 0; bitId < bitsPerBinNum; bitId++) {
                                if (firstGroupConstituent.element[bitId] != secondGroupConstituent.element[bitId]) {
                                    if (++countOfDifferentBits > 1) {
                                        isConstituentsMergeable = false;
                                        break;
                                    }
                                    isConstituentsMergeable = true;
                                    implicantsGroupKey += "x";
                                } else {
                                    implicantsGroupKey += "_";
                                }
                            }
                            if (isConstituentsMergeable) {
                                
                            }
                        }
                    }
                }
            } // иначе?
            
            
            // 3) Производим попарное сравнение элементов соседних групп, при этом помечаем те 
            //    бинарные числа, которые нашли свою пару
            // 4) С помощью метода Петрика находим тупиковую ДНФ
        }

        static uint CountSymbolsInString(string str,char symbolToCount) {
            uint countOfSymbols = 0;

            foreach(char symbol in str) {
                if (symbol == symbolToCount) {
                    countOfSymbols++;
                }
            } 
            
            return countOfSymbols;
        }

        static void OutputListInConsole<T>(List<T> outputList) {
            foreach(T listEl in outputList) {
                Console.WriteLine(listEl);
            }
        }

        // static void OutputDictionaryInConsole<T, K>(Dictionary<T, K> outputDictionary) {
            
        // }
    }
}