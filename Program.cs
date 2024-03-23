using System.Collections.Specialized;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Text.RegularExpressions;
using NewCollections.MultiValueDictionary;
using NewValues.MarkableValue;

namespace MLITA {

    class MLITA_LAB2 {
        const char MERGED_BITS_SYMBOL = 'X';
        const char BIT_ONE_SYMBOL = '1';
        const char BIT_ZERO_SYMBOL = '0';

        static void Main(string[] args) {
            Console.WriteLine("Enter list of numbers of unit sets of a Boolean function:");
            
            var numbersString = String.Empty;
            while (numbersString == string.Empty) {
            numbersString = Console.ReadLine();
            }

            FindMinDNF(MakeNumbersListFromString(numbersString));
        }

        static List<int> MakeNumbersListFromString(string numbersString) {
            var regexForNumbers = new Regex("\\d+"); // маска чисел для строки 
            var numbersList = regexForNumbers.Matches(numbersString).Cast<Match>().Select(matchEl => int.Parse(matchEl.Value)).ToList();

            return numbersList;
        }

        static List<String> ConvertNumbersFromDecToBin(List<int> decNumbersList, out int bitsPerBinNum) {
            var logOfMaxDecNum = Math.Log2(decNumbersList.Max());
            var numberOfBits = (int)Math.Ceiling(logOfMaxDecNum);

            if (numberOfBits == logOfMaxDecNum) {
                numberOfBits++;
            }
            var binaryNumbersList = decNumbersList.Select(decNum => {
                var binaryNumStr = Convert.ToString(decNum, 2);
                if (binaryNumStr.Length < numberOfBits) {
                    binaryNumStr = new String(BIT_ZERO_SYMBOL, (int)numberOfBits-binaryNumStr.Length) + binaryNumStr;
                } 
                return binaryNumStr;
                }).ToList();

            bitsPerBinNum = numberOfBits;
            return binaryNumbersList;
        }

        static void FindMinDNF(List<int> decConstituentsOfOnes) {
            var binConstituentsOfOnes = ConvertNumbersFromDecToBin(decConstituentsOfOnes, out _);
            Console.WriteLine();
            OutputListInConsole(binConstituentsOfOnes);
            Console.WriteLine();
            var PrimeImplicants = GetPrimeImplicants(binConstituentsOfOnes);
            OutputListInConsole(PrimeImplicants);
            // 3) Производим попарное сравнение элементов соседних групп, при этом помечаем те 
            //    бинарные числа, которые нашли свою пару
            // 4) С помощью метода Петрика находим тупиковую ДНФ
        }

        static List<string> GetPrimeImplicants(List<string> binConstituentsOfOnes) {
            List<string> primeImplicants = new();
            var bitsPerElement = binConstituentsOfOnes[0].Length;

            // пахнет рекурсией...
            var mergeElements = binConstituentsOfOnes;
            do {

            MultiValueDictionary<uint, MarkableValue<string>> mergeElementsGroups = new();
            foreach(var mergeElement in mergeElements) {
                var numberOfOnesInElement = CountSymbolsInString(mergeElement, BIT_ONE_SYMBOL);
                mergeElementsGroups.Add(numberOfOnesInElement, new MarkableValue<string>(mergeElement));
            }
            List<uint> mergeElementsGroupsKeys = mergeElementsGroups.Keys.ToList();
            mergeElementsGroupsKeys.Sort();

            List<string> implicants = new();
            // Если получилось больше, чем одна, групп, то производим их склейку
            if (mergeElementsGroups.Count > 1) {
                int currentMergeElementsGroupKeyId = 0;
                // цикл склейки элементов групп
                while (currentMergeElementsGroupKeyId < mergeElementsGroupsKeys.Count - 1) {
                    var currentMergeElementsGroup = mergeElementsGroups[mergeElementsGroupsKeys[currentMergeElementsGroupKeyId]]; 
                    var nextMergeElementsGroup = mergeElementsGroups[mergeElementsGroupsKeys[currentMergeElementsGroupKeyId + 1]];
                    // цикл склейки элементов текущей и следующей группы
                    for (int curGroupElemId = 0; curGroupElemId < currentMergeElementsGroup.Count; curGroupElemId++) {
                        for (int nextGroupElemId = 0; nextGroupElemId < nextMergeElementsGroup.Count; nextGroupElemId++) {
                            var curGroupMergeEl = currentMergeElementsGroup[curGroupElemId].value;
                            var nextGroupMergeEl = nextMergeElementsGroup[nextGroupElemId].value;

                            bool isElementsMergeable = false;
                            int countOfDifferentBits = 0;

                            string mergedElement = curGroupMergeEl; // инициализируем склеенный элемент одним из исходных элементов
                            for (int bitId = 0; bitId < bitsPerElement; bitId++) {
                                if (curGroupMergeEl[bitId] != nextGroupMergeEl[bitId]) {
                                    if (countOfDifferentBits > 1 || curGroupMergeEl[bitId] == MERGED_BITS_SYMBOL || nextGroupMergeEl[bitId] == MERGED_BITS_SYMBOL ) { // склеиваем только те элементы, которые отличаются на один бит и имеют "x"-ы в одинаковых позициях
                                        isElementsMergeable = false;
                                        break;
                                    }
                                    mergedElement = mergedElement.Remove(bitId, 1).Insert(bitId, new String(MERGED_BITS_SYMBOL, 1));
                                    isElementsMergeable = true;
                                    countOfDifferentBits++;
                                }
                            } 
                            if (isElementsMergeable) {
                                bool isNecessaryToAdd = true;
                                foreach (var implicant in implicants) {
                                    if (mergedElement == implicant) {
                                        isNecessaryToAdd = false;
                                        break;
                                    }
                                }
                                if (isNecessaryToAdd) {
                                    implicants.Add(mergedElement);
                                }
                                mergeElementsGroups[mergeElementsGroupsKeys[currentMergeElementsGroupKeyId]][curGroupElemId].SetMark();
                                mergeElementsGroups[mergeElementsGroupsKeys[currentMergeElementsGroupKeyId + 1]][nextGroupElemId].SetMark();
                            }
                        }
                    }
                    currentMergeElementsGroupKeyId++; // переходим к следующей группе
                }
            }
            foreach (var mergeElementsGroupKey in mergeElementsGroupsKeys) {
                foreach (var mergeElement in mergeElementsGroups[mergeElementsGroupKey]) {
                    if (!mergeElement.Marked) {
                        primeImplicants.Add(mergeElement.value);
                    }
                }
            }
            mergeElements = implicants;
            
            } while(mergeElements.Count != 0);

            foreach (var mergeEl in mergeElements) {
                primeImplicants.Add(mergeEl);
            }
            return primeImplicants;
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
    }
}