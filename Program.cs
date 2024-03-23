using System.Collections.Specialized;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using NewCollections.MultiValueDictionary;
using NewValues.MarkableValue;
using SymbolsConstants;

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

         static void FindMinDNF(List<int> decConstituentsOfOnes) {
            var binConstituentsOfOnes = ConvertNumbersFromDecToBin(decConstituentsOfOnes, out _);
            var primeImplicants = GetPrimeImplicants(binConstituentsOfOnes);
            var implicantTable = MakeImplicantTable(primeImplicants, binConstituentsOfOnes);
            Console.WriteLine();
            DrawImplicantTable(implicantTable, true);
            Console.WriteLine();
            var CNF = MakeCNFFromImplicantTable(implicantTable);
        }

        static string MinimizeCNF(string CNF) {
            string minCNF = string.Empty;
            
            return minCNF;
        }

        static string MakeCNFFromImplicantTable(Dictionary<string, Dictionary<string, bool>> implicantTable) {
            var implicants = implicantTable.Keys.ToList();
            var constituents = implicantTable[implicants[0]].Keys.ToList();
            Dictionary<char, Dictionary<string, bool>> namedImplicantTable = new();

            List<char> namedImplicantTableKeys = new();
            char variableChar = 'a';
            foreach (var implicant in implicants) {
                namedImplicantTable.Add(variableChar, new());
                namedImplicantTableKeys.Add(variableChar);
                foreach (var constituent in constituents) {
                    namedImplicantTable[variableChar].Add(constituent, implicantTable[implicant][constituent]);
                }
                variableChar++;
            }

            string CNF = string.Empty;
            foreach (var constituent in constituents) {
                if (CNF != string.Empty) {
                    CNF += MlitaSymbols.DISJUNCTION_SYMBOL;
                }
                List<char> operandsList = new();
                foreach (var variable in namedImplicantTableKeys) {
                    if (namedImplicantTable[variable][constituent]) {
                        operandsList.Add(variable);
                    }
                }
                if (operandsList.Count == 1) {
                    CNF += operandsList[0];
                } else if (operandsList.Count > 1) {
                    CNF += MlitaSymbols.OPENING_PARENTHESIS_SYMBOL;
                    for (int operandId = 0; operandId < operandsList.Count; operandId++)  {
                        CNF += operandsList[operandId];
                        if (operandId != operandsList.Count - 1) {
                            CNF += MlitaSymbols.CONJUNCTION_SYMBOL;
                        }
                    }
                    CNF += MlitaSymbols.CLOSING_PARENTHESIS_SYMBOL;
                }
            }

            return CNF;
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
                    binaryNumStr = new String(MlitaSymbols.BIT_ZERO_SYMBOL, (int)numberOfBits-binaryNumStr.Length) + binaryNumStr;
                } 
                return binaryNumStr;
                }).ToList();

            bitsPerBinNum = numberOfBits;
            return binaryNumbersList;
        }

        static void DrawImplicantTable(Dictionary<string, Dictionary<string, bool>> implicantTable, bool namedImplicants = false) {
            var primeImplicants = implicantTable.Keys.ToList();
            var constituentsOfOne = implicantTable[primeImplicants[0]].Keys.ToList();
            int bitsPerNum = constituentsOfOne[0].Length;
            int spaceArountCell = 1;
            Console.Write(new string(MlitaSymbols.ROW_SYMBOL, bitsPerNum + spaceArountCell*2) + MlitaSymbols.COL_SYMBOL);
            foreach (var constituent in constituentsOfOne) {
                Console.Write(new String(MlitaSymbols.ROW_SYMBOL, spaceArountCell) + constituent + new String(MlitaSymbols.ROW_SYMBOL, spaceArountCell) + MlitaSymbols.COL_SYMBOL);
            }
            if (namedImplicants) {
                Console.Write(new string(MlitaSymbols.ROW_SYMBOL, 1 + spaceArountCell*2) + MlitaSymbols.COL_SYMBOL);
            }
            Console.WriteLine();
            int row = 0, col = 0;
            char currentImplicantName = 'a';
            foreach (var implicant in primeImplicants) {
                Console.Write(new String(MlitaSymbols.ROW_SYMBOL, spaceArountCell) + implicant + new String(MlitaSymbols.ROW_SYMBOL, spaceArountCell) + MlitaSymbols.COL_SYMBOL);
                foreach(var constituent in constituentsOfOne) {
                    Console.Write(new String(MlitaSymbols.ROW_SYMBOL, spaceArountCell + (int)Math.Ceiling((Convert.ToDouble(bitsPerNum)-1)/2)));
                    if (implicantTable[implicant][constituent]) {
                        Console.Write(MlitaSymbols.COVERED_CONSTITUENT_SYMBOL);
                    } else {
                        Console.Write(MlitaSymbols.UNCOVERED_CONSTITUENT_SYMBOL);
                    }
                    Console.Write(new String(MlitaSymbols.ROW_SYMBOL, spaceArountCell + (int)Math.Floor((Convert.ToDouble(bitsPerNum)-1)/2)) + MlitaSymbols.COL_SYMBOL);
                    col++;
                }
                Console.Write(new string(MlitaSymbols.ROW_SYMBOL, spaceArountCell) + new string(currentImplicantName, 1) + new string(MlitaSymbols.ROW_SYMBOL, spaceArountCell) + MlitaSymbols.COL_SYMBOL);
                Console.WriteLine();
                col = 0;
                row++;
                currentImplicantName++;
            }
        }

        static Dictionary<string, Dictionary<string, bool>> MakeImplicantTable(List<string> primeImplicants, List<string> constituentsOfOne) {
            Dictionary<string, Dictionary<string, bool>> implicantTable = new();
            var bitsPerNum = constituentsOfOne[0].Length;
            foreach (var implicant in primeImplicants) {
                implicantTable.Add(implicant, new Dictionary<string, bool>());
                foreach (var constituent in constituentsOfOne) {
                    bool isConstituentCovered = true;
                    for (int bitId = 0; bitId < bitsPerNum; bitId++) {
                        if (implicant[bitId] == MlitaSymbols.MERGED_BITS_SYMBOL) {
                            continue;
                        } else if (implicant[bitId] != constituent[bitId]){
                            isConstituentCovered = false;
                        }
                    }
                    implicantTable[implicant].Add(constituent, isConstituentCovered);
                }
            }
            return implicantTable;
        }

        static List<string> GetPrimeImplicants(List<string> binConstituentsOfOnes) {
            List<string> primeImplicants = new();
            var bitsPerElement = binConstituentsOfOnes[0].Length;
            var mergeElements = binConstituentsOfOnes;
            do {

            MultiValueDictionary<uint, MarkableValue<string>> mergeElementsGroups = new();
            foreach(var mergeElement in mergeElements) {
                var numberOfOnesInElement = CountSymbolsInString(mergeElement, MlitaSymbols.BIT_ONE_SYMBOL);
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
                                    if (countOfDifferentBits > 1 || curGroupMergeEl[bitId] == MlitaSymbols.MERGED_BITS_SYMBOL || nextGroupMergeEl[bitId] == MlitaSymbols.MERGED_BITS_SYMBOL ) { // склеиваем только те элементы, которые отличаются на один бит и имеют "x"-ы в одинаковых позициях
                                        isElementsMergeable = false;
                                        break;
                                    }
                                    mergedElement = mergedElement.Remove(bitId, 1).Insert(bitId, new String(MlitaSymbols.MERGED_BITS_SYMBOL, 1));
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