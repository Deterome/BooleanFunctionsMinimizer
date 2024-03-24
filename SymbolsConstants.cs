namespace SymbolsConstants {
    struct MlitaSymbols {
        public const char DNF_VARIABLE_SYMBOL = 'x';
        public const char INVERSE_SYMBOL = (char)773;
        public const char BEGIN_OF_IMPLICANTS_NAME_ALPHABET = 'a';
        public const char MERGED_BITS_SYMBOL = '-';
        public const char BIT_ONE_SYMBOL = '1';
        public const char BIT_ZERO_SYMBOL = '0';
        public const char ROW_SYMBOL = '_';
        public const char COL_SYMBOL = '|';
        public const char COVERED_CONSTITUENT_SYMBOL = 'X';
        public const char UNCOVERED_CONSTITUENT_SYMBOL = ROW_SYMBOL;
        public const char OPENING_PARENTHESIS_SYMBOL = '(';
        public const char CLOSING_PARENTHESIS_SYMBOL = ')';
        public const char DISJUNCTION_SYMBOL = '+';
        public const char CONJUNCTION_SYMBOL = '*';
        static public string GetLowerIndex(int index) {
            var lowerIndex = string.Empty; 
            var ordinaryIndex = Convert.ToString(index);

            foreach (var ordinaryIndexSymbol in ordinaryIndex) {
                lowerIndex += (char)((int)ordinaryIndexSymbol - (int)'1' + '₁');
            }

            return lowerIndex;
        }
    }
        
}