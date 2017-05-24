using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net;
using System.IO;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Analyzer = Lucene.Net.Analysis.Analyzer;
using StopFilter = Lucene.Net.Analysis.StopFilter;
using TokenStream = Lucene.Net.Analysis.TokenStream;
using Lucene.Net.Index;

namespace LuceneTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //TEst changes
            string term = Console.ReadLine();
            string searchableTerm = term + "*";
            string dirPath = @"E:\Downloads\ProductsIndex (2)\ProductsIndex";
            DirectoryInfo di = new DirectoryInfo(dirPath);
            float? o = null;
            decimal? k =(decimal?) o;
            double p = 1.45;
            IndexReader reader = DirectoryReader.Open(dirPath);
            IndexSearcher indexSearcher = new IndexSearcher(reader);
               // var parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_29, "CategoryId");

            var analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer();
            Lucene.Net.Search.BooleanQuery bq = new Lucene.Net.Search.BooleanQuery();
            Lucene.Net.Search.MatchAllDocsQuery query=new MatchAllDocsQuery();
            
            //var parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_29, "ID", analyzer);
            //query = parser.Parse("123185");
            //var res = indexSearcher.Search(query);
            //int c = res.Length();
            //query.SetBoost(100); // boost score to make this field more relevant
            //bq.Add(query, Lucene.Net.Search.BooleanClause.Occur.MUST);


            /*parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_CURRENT, "Sku", analyzer);
            query = parser.Parse(searchableTerm);
            query.SetBoost(10);
            bq.Add(query, Lucene.Net.Search.BooleanClause.Occur.SHOULD);

           parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Description.LongDescription", analyzer);
           query = parser.Parse(searchableTerm);
            query.SetBoost(7);
            bq.Add(query, Lucene.Net.Search.BooleanClause.Occur.SHOULD);

            parser = new Lucene.Net.QueryParsers.QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Description.ShortDescription", analyzer);
            query = parser.Parse(searchableTerm);
            query.SetBoost(7);
            bq.Add(query, Lucene.Net.Search.BooleanClause.Occur.SHOULD);
            */
            /* Open the directory to be searched... */
            var productsList = new Dictionary<string,float>();
            /*SortField[] sortFields = new[] { 
                new SortField("Description.LongDescription",Lucene.Net.Search.SortField.STRING_VAL), 
                new SortField("Description.ShortDescription",Lucene.Net.Search.SortField.STRING_VAL) 
            };
            Sort sort = new Sort(sortFields);*/
            using (var directory = Lucene.Net.Store.FSDirectory.Open(di))
            {
                using (var searcher = new Lucene.Net.Search.IndexSearcher(Lucene.Net.Index.IndexReader.Open(directory, true)))
                {
                    var hits2= searcher.Search(query);
                    int hitsCount = hits2.Length();

                    for (int i = 0; i < hits2.Length();i++ )
                    {
                        Document doc = hits2.Doc(i);
                        string prodName = doc.Get("DisplayName");
                        productsList.Add(prodName, doc.GetBoost());
                    }
                    var r = productsList.OrderByDescending(x => x.Key.StartsWith(term,StringComparison.InvariantCultureIgnoreCase)).ThenByDescending(x=>x.Key.ToLower().Contains(term.ToLower())).ToList();//.ToDictionary(x=>new KeyValuePair<string,float>(x.Key,x.Value));
                    
                    var sortBy = new Lucene.Net.Search.Sort(new Lucene.Net.Search.SortField("DisplayName", Lucene.Net.Search.SortField.STRING_VAL, true));
                    Lucene.Net.Search.TopDocsCollector collector =  Lucene.Net.Search.TopScoreDocCollector.create(10000,true); // default is relevance
                    collector = Lucene.Net.Search.TopFieldCollector.create(
                        sortBy,
                        10000,  
                        false,
                        false,
                        false,
                        false);
                    //break;*
                     searcher.Search(bq, collector);
                     var hits = collector.TopDocs().ScoreDocs ;
                    /* execute the query and return the top hits */

                    int l = hits.Length;
                    for (int i = 0; i < l; i++)
                    {
                        Document doc =searcher.Doc(hits[i].doc);
                        string prodName = doc.Get("DisplayName");
                        productsList.Add(prodName,hits[i].score);
                    }
                }
            }
            Console.ReadLine();
        }
    }
}

/*
 * Copyright 2004 The Apache Software Foundation
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Lucene.Net.Analysis.RU
{
	
	/// <summary> Analyzer for Russian language. Supports an external list of stopwords (words that
	/// will not be indexed at all).
	/// A default set of stopwords is used unless an alternative list is specified.
	/// 
	/// </summary>
	/// <author>   Boris Okner, b.okner@rogers.com
	/// </author>
	/// <version>  $Id: RussianAnalyzer.java,v 1.7 2004/03/29 22:48:01 cutting Exp $
	/// </version>
	public sealed class RussianAnalyzer : Analyzer
	{
		// letters
		private static char A = (char) (0);
		private static char B = (char) (1);
		private static char V = (char) (2);
		private static char G = (char) (3);
		private static char D = (char) (4);
		private static char E = (char) (5);
		private static char ZH = (char) (6);
		private static char Z = (char) (7);
		private static char I = (char) (8);
		private static char I_ = (char) (9);
		private static char K = (char) (10);
		private static char L = (char) (11);
		private static char M = (char) (12);
		private static char N = (char) (13);
		private static char O = (char) (14);
		private static char P = (char) (15);
		private static char R = (char) (16);
		private static char S = (char) (17);
		private static char T = (char) (18);
		private static char U = (char) (19);
		private static char F = (char) (20);
		private static char X = (char) (21);
		private static char TS = (char) (22);
		private static char CH = (char) (23);
		private static char SH = (char) (24);
		private static char SHCH = (char) (25);
		private static char HARD = (char) (26);
		private static char Y = (char) (27);
		private static char SOFT = (char) (28);
		private static char AE = (char) (29);
		private static char IU = (char) (30);
		private static char IA = (char) (31);
		
		/// <summary> List of typical Russian stopwords.</summary>
		private static char[][] RUSSIAN_STOP_WORDS = new char[][]{new char[]{A}, new char[]{B, E, Z}, new char[]{B, O, L, E, E}, new char[]{B, Y}, new char[]{B, Y, L}, new char[]{B, Y, L, A}, new char[]{B, Y, L, I}, new char[]{B, Y, L, O}, new char[]{B, Y, T, SOFT}, new char[]{V}, new char[]{V, A, M}, new char[]{V, A, S}, new char[]{V, E, S, SOFT}, new char[]{V, O}, new char[]{V, O, T}, new char[]{V, S, E}, new char[]{V, S, E, G, O}, new char[]{V, S, E, X}, new char[]{V, Y}, new char[]{G, D, E}, new char[]{D, A}, new char[]{D, A, ZH, E}, new char[]{D, L, IA}, new char[]{D, O}, new char[]{E, G, O}, new char[]{E, E}, new char[]{E, I_}, new char[]{E, IU}, new char[]{E, S, L, I}, new char[]{E, S, T, SOFT}, new char[]{E, SHCH, E}, new char[]{ZH, E}, new char[]{Z, A}, new char[]{Z, D, E, S, SOFT}, new char[]{I}, new char[]{I, Z}, new char[]{I, L, I}, new char[]{I, M}, new char[]{I, X}, new char[]{K}, new char[]{K, A, K}, new char[]{K, O}, new char[]{K, O, G, D, A}, new char[]{K, T, O}, new char[]{L, I}, new char[]{L, I, B, O}, new char[]{M, N, E}, new char[]{M, O, ZH, E, T}, new char[]{M, Y}, new char[]{N, A}, new char[]{N, A, D, O}, new char[]{N, A, SH}, new char[]{N, E}, new char[]{N, E, G, O}, new char[]{N, E, E}, new char[]{N, E, T}, new char[]{N, I}, new char[]{N, I, X}, new char[]{N, O}, new char[]{N, U}, new char[]{O}, new char[]{O, B}, new char[]{O, D, N, A, K, O}, new char[]{O, N}, new char[]{O, N, A}, new char[]{O, N, I}, new char[]{O, N, O}, new char[]{O, T}, new char[]{O, CH, E, N, SOFT}, new char[]{P, O}, new char[]{P, O, D}, new char[]{P, R, I}, new char[]{S}, new char[]{S, O}, new char[]{T, A, K}, new char[]{T, A, K, ZH, E}, new char[]{T, A, K, O, I_}, new char[]{T, A, M}, new char[]{T, E}, new char[]{T, E, M}, new char[]{T, O}, new char[]{T, O, G, O}, new char[]{T, O, ZH, E}, new char[]{T, O, I_}, new char[]{T, O, L, SOFT, K, O}, new char[]{T, O, M}, new char[]{T, Y}, new char[]{U}, new char[]{U, ZH, E}, new char[]{X, O, T, IA}, new char[]{CH, E, G, O}, new char[]{CH, E, I_}, new char[]{CH, E, M}, 
			new char[]{CH, T, O}, new char[]{CH, T, O, B, Y}, new char[]{CH, SOFT, E}, new char[]{CH, SOFT, IA}, new char[]{AE, T, A}, new char[]{AE, T, I}, new char[]{AE, T, O}, new char[]{IA}};
		
		/// <summary> Contains the stopwords used with the StopFilter.</summary>
		private System.Collections.Hashtable stopSet = new System.Collections.Hashtable();
		
		/// <summary> Charset for Russian letters.
		/// Represents encoding for 32 lowercase Russian letters.
		/// Predefined charsets can be taken from RussianCharSets class
		/// </summary>
		private char[] charset;
		
		
		public RussianAnalyzer()
		{
			charset = RussianCharsets.UnicodeRussian;
			stopSet = StopFilter.MakeStopSet(makeStopWords(RussianCharsets.UnicodeRussian));
		}
		
		/// <summary> Builds an analyzer.</summary>
		public RussianAnalyzer(char[] charset)
		{
			this.charset = charset;
			stopSet = StopFilter.MakeStopSet(makeStopWords(charset));
		}
		
		/// <summary> Builds an analyzer with the given stop words.</summary>
		public RussianAnalyzer(char[] charset, System.String[] stopwords)
		{
			this.charset = charset;
			stopSet = StopFilter.MakeStopSet(stopwords);
		}
		
		// Takes russian stop words and translates them to a String array, using
		// the given charset
		private static System.String[] makeStopWords(char[] charset)
		{
			System.String[] res = new System.String[RUSSIAN_STOP_WORDS.Length];
			for (int i = 0; i < res.Length; i++)
			{
				char[] theStopWord = RUSSIAN_STOP_WORDS[i];
				// translate the word,using the charset
				System.Text.StringBuilder theWord = new System.Text.StringBuilder();
				for (int j = 0; j < theStopWord.Length; j++)
				{
					theWord.Append(charset[theStopWord[j]]);
				}
				res[i] = theWord.ToString();
			}
			return res;
		}
		
		/// <summary> Builds an analyzer with the given stop words.</summary>
		/// <todo>  create a Set version of this ctor </todo>
		public RussianAnalyzer(char[] charset, System.Collections.Hashtable stopwords)
		{
			this.charset = charset;
			stopSet = new System.Collections.Hashtable(new System.Collections.Hashtable(stopwords));
		}
		
		/// <summary> Creates a TokenStream which tokenizes all the text in the provided Reader.
		/// 
		/// </summary>
		/// <returns>  A TokenStream build from a RussianLetterTokenizer filtered with
		/// RussianLowerCaseFilter, StopFilter, and RussianStemFilter
		/// </returns>
		public override TokenStream TokenStream(System.String fieldName, System.IO.TextReader reader)
		{
			TokenStream result = new RussianLetterTokenizer(reader, charset);
			result = new RussianLowerCaseFilter(result, charset);
			result = new StopFilter(result, stopSet);
			result = new RussianStemFilter(result, charset);
			return result;
		}
	}

    /// <summary> RussianCharsets class contains encodings schemes (charsets) and toLowerCase() method implementation
    /// for russian characters in Unicode, KOI8 and CP1252.
    /// Each encoding scheme contains lowercase (positions 0-31) and uppercase (position 32-63) characters.
    /// One should be able to add other encoding schemes (like ISO-8859-5 or customized) by adding a new charset
    /// and adding logic to toLowerCase() method for that charset.
    /// 
    /// </summary>
    /// <author>   Boris Okner, b.okner@rogers.com
    /// </author>
    /// <version>  $Id: RussianCharsets.java,v 1.3 2004/03/29 22:48:01 cutting Exp $
    /// </version>
    public class RussianCharsets
    {
        // Unicode Russian charset (lowercase letters only)
        public static char[] UnicodeRussian = new char[] { '\u0430', '\u0431', '\u0432', '\u0433', '\u0434', '\u0435', '\u0436', '\u0437', '\u0438', '\u0439', '\u043A', '\u043B', '\u043C', '\u043D', '\u043E', '\u043F', '\u0440', '\u0441', '\u0442', '\u0443', '\u0444', '\u0445', '\u0446', '\u0447', '\u0448', '\u0449', '\u044A', '\u044B', '\u044C', '\u044D', '\u044E', '\u044F', '\u0410', '\u0411', '\u0412', '\u0413', '\u0414', '\u0415', '\u0416', '\u0417', '\u0418', '\u0419', '\u041A', '\u041B', '\u041C', '\u041D', '\u041E', '\u041F', '\u0420', '\u0421', '\u0422', '\u0423', '\u0424', '\u0425', '\u0426', '\u0427', '\u0428', '\u0429', '\u042A', '\u042B', '\u042C', '\u042D', '\u042E', '\u042F' };

        // KOI8 charset
        public static char[] KOI8 = new char[] { (char)(0xc1), (char)(0xc2), (char)(0xd7), (char)(0xc7), (char)(0xc4), (char)(0xc5), (char)(0xd6), (char)(0xda), (char)(0xc9), (char)(0xca), (char)(0xcb), (char)(0xcc), (char)(0xcd), (char)(0xce), (char)(0xcf), (char)(0xd0), (char)(0xd2), (char)(0xd3), (char)(0xd4), (char)(0xd5), (char)(0xc6), (char)(0xc8), (char)(0xc3), (char)(0xde), (char)(0xdb), (char)(0xdd), (char)(0xdf), (char)(0xd9), (char)(0xd8), (char)(0xdc), (char)(0xc0), (char)(0xd1), (char)(0xe1), (char)(0xe2), (char)(0xf7), (char)(0xe7), (char)(0xe4), (char)(0xe5), (char)(0xf6), (char)(0xfa), (char)(0xe9), (char)(0xea), (char)(0xeb), (char)(0xec), (char)(0xed), (char)(0xee), (char)(0xef), (char)(0xf0), (char)(0xf2), (char)(0xf3), (char)(0xf4), (char)(0xf5), (char)(0xe6), (char)(0xe8), (char)(0xe3), (char)(0xfe), (char)(0xfb), (char)(0xfd), (char)(0xff), (char)(0xf9), (char)(0xf8), (char)(0xfc), (char)(0xe0), (char)(0xf1) };

        // CP1251 eharset
        public static char[] CP1251 = new char[] { (char)(0xE0), (char)(0xE1), (char)(0xE2), (char)(0xE3), (char)(0xE4), (char)(0xE5), (char)(0xE6), (char)(0xE7), (char)(0xE8), (char)(0xE9), (char)(0xEA), (char)(0xEB), (char)(0xEC), (char)(0xED), (char)(0xEE), (char)(0xEF), (char)(0xF0), (char)(0xF1), (char)(0xF2), (char)(0xF3), (char)(0xF4), (char)(0xF5), (char)(0xF6), (char)(0xF7), (char)(0xF8), (char)(0xF9), (char)(0xFA), (char)(0xFB), (char)(0xFC), (char)(0xFD), (char)(0xFE), (char)(0xFF), (char)(0xC0), (char)(0xC1), (char)(0xC2), (char)(0xC3), (char)(0xC4), (char)(0xC5), (char)(0xC6), (char)(0xC7), (char)(0xC8), (char)(0xC9), (char)(0xCA), (char)(0xCB), (char)(0xCC), (char)(0xCD), (char)(0xCE), (char)(0xCF), (char)(0xD0), (char)(0xD1), (char)(0xD2), (char)(0xD3), (char)(0xD4), (char)(0xD5), (char)(0xD6), (char)(0xD7), (char)(0xD8), (char)(0xD9), (char)(0xDA), (char)(0xDB), (char)(0xDC), (char)(0xDD), (char)(0xDE), (char)(0xDF) };

        public static char ToLowerCase(char letter, char[] charset)
        {
            if (charset == UnicodeRussian)
            {
                if (letter >= '\u0430' && letter <= '\u044F')
                {
                    return letter;
                }
                if (letter >= '\u0410' && letter <= '\u042F')
                {
                    return (char)(letter + 32);
                }
            }

            if (charset == KOI8)
            {
                if (letter >= 0xe0 && letter <= 0xff)
                {
                    return (char)(letter - 32);
                }
                if (letter >= 0xc0 && letter <= 0xdf)
                {
                    return letter;
                }
            }

            if (charset == CP1251)
            {
                if (letter >= 0xC0 && letter <= 0xDF)
                {
                    return (char)(letter + 32);
                }
                if (letter >= 0xE0 && letter <= 0xFF)
                {
                    return letter;
                }
            }

            return System.Char.ToLower(letter);
        }
    }

    /// <summary> A RussianLetterTokenizer is a tokenizer that extends LetterTokenizer by additionally looking up letters
    /// in a given "russian charset". The problem with LeterTokenizer is that it uses Character.isLetter() method,
    /// which doesn't know how to detect letters in encodings like CP1252 and KOI8
    /// (well-known problems with 0xD7 and 0xF7 chars)
    /// 
    /// </summary>
    /// <author>   Boris Okner, b.okner@rogers.com
    /// </author>
    /// <version>  $Id: RussianLetterTokenizer.java,v 1.3 2004/03/29 22:48:01 cutting Exp $
    /// </version>

    public class RussianLetterTokenizer : CharTokenizer
    {
        /// <summary>Construct a new LetterTokenizer. </summary>
        private char[] charset;

        public RussianLetterTokenizer(System.IO.TextReader in_Renamed, char[] charset)
            : base(in_Renamed)
        {
            this.charset = charset;
        }

        /// <summary> Collects only characters which satisfy
        /// {@link Character#isLetter(char)}.
        /// </summary>
        protected /*internal*/ override bool IsTokenChar(char c)
        {
            if (System.Char.IsLetter(c))
                return true;
            for (int i = 0; i < charset.Length; i++)
            {
                if (c == charset[i])
                    return true;
            }
            return false;
        }
    }

    public sealed class RussianLowerCaseFilter : TokenFilter
    {
        internal char[] charset;

        public RussianLowerCaseFilter(TokenStream in_Renamed, char[] charset)
            : base(in_Renamed)
        {
            this.charset = charset;
        }

        public override Token Next()
        {
            Token t = input.Next();

            if (t == null)
                return null;

            System.String txt = t.TermText();

            char[] chArray = txt.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                chArray[i] = RussianCharsets.ToLowerCase(chArray[i], charset);
            }

            System.String newTxt = new System.String(chArray);
            // create new token
            Token newToken = new Token(newTxt, t.StartOffset(), t.EndOffset());

            return newToken;
        }
    }

    /// <summary> A filter that stems Russian words. The implementation was inspired by GermanStemFilter.
    /// The input should be filtered by RussianLowerCaseFilter before passing it to RussianStemFilter ,
    /// because RussianStemFilter only works  with lowercase part of any "russian" charset.
    /// 
    /// </summary>
    /// <author>     Boris Okner, b.okner@rogers.com
    /// </author>
    /// <version>    $Id: RussianStemFilter.java,v 1.5 2004/03/29 22:48:01 cutting Exp $
    /// </version>
    public sealed class RussianStemFilter : TokenFilter
    {
        /// <summary> The actual token in the input stream.</summary>
        private Token token = null;
        private RussianStemmer stemmer = null;

        public RussianStemFilter(TokenStream in_Renamed, char[] charset)
            : base(in_Renamed)
        {
            stemmer = new RussianStemmer(charset);
        }

        /// <returns>  Returns the next token in the stream, or null at EOS
        /// </returns>
        public override Token Next()
        {
            if ((token = input.Next()) == null)
            {
                return null;
            }
            else
            {
                System.String s = stemmer.Stem(token.TermText());
                if (!s.Equals(token.TermText()))
                {
                    return new Token(s, token.StartOffset(), token.EndOffset(), token.Type());
                }
                return token;
            }
        }

        /// <summary> Set a alternative/custom RussianStemmer for this filter.</summary>
        public void SetStemmer(RussianStemmer stemmer)
        {
            if (stemmer != null)
            {
                this.stemmer = stemmer;
            }
        }
    }

    /// <summary> Russian stemming algorithm implementation (see http://snowball.sourceforge.net for detailed description).
    /// 
    /// </summary>
    /// <author>   Boris Okner, b.okner@rogers.com
    /// </author>
    /// <version>  $Id: RussianStemmer.java,v 1.5 2004/03/29 22:48:01 cutting Exp $
    /// </version>
    public class RussianStemmer
    {
        private char[] charset;

        // positions of RV, R1 and R2 respectively
        private int RV, R1, R2;

        // letters
        private static char A = (char)(0);
        private static char B = (char)(1);
        private static char V = (char)(2);
        private static char G = (char)(3);
        private static char D = (char)(4);
        private static char E = (char)(5);
        private static char ZH = (char)(6);
        private static char Z = (char)(7);
        private static char I = (char)(8);
        private static char I_ = (char)(9);
        private static char K = (char)(10);
        private static char L = (char)(11);
        private static char M = (char)(12);
        private static char N = (char)(13);
        private static char O = (char)(14);
        private static char P = (char)(15);
        private static char R = (char)(16);
        private static char S = (char)(17);
        private static char T = (char)(18);
        private static char U = (char)(19);
        private static char F = (char)(20);
        private static char X = (char)(21);
        private static char TS = (char)(22);
        private static char CH = (char)(23);
        private static char SH = (char)(24);
        private static char SHCH = (char)(25);
        private static char HARD = (char)(26);
        private static char Y = (char)(27);
        private static char SOFT = (char)(28);
        private static char AE = (char)(29);
        private static char IU = (char)(30);
        private static char IA = (char)(31);

        // stem definitions
        private static char[] vowels = new char[] { A, E, I, O, U, Y, AE, IU, IA };

        private static char[][] perfectiveGerundEndings1 = new char[][] { new char[] { V }, new char[] { V, SH, I }, new char[] { V, SH, I, S, SOFT } };

        private static char[][] perfectiveGerund1Predessors = new char[][] { new char[] { A }, new char[] { IA } };

        private static char[][] perfectiveGerundEndings2 = new char[][] { new char[] { I, V }, new char[] { Y, V }, new char[] { I, V, SH, I }, new char[] { Y, V, SH, I }, new char[] { I, V, SH, I, S, SOFT }, new char[] { Y, V, SH, I, S, SOFT } };

        private static char[][] adjectiveEndings = new char[][] { new char[] { E, E }, new char[] { I, E }, new char[] { Y, E }, new char[] { O, E }, new char[] { E, I_ }, new char[] { I, I_ }, new char[] { Y, I_ }, new char[] { O, I_ }, new char[] { E, M }, new char[] { I, M }, new char[] { Y, M }, new char[] { O, M }, new char[] { I, X }, new char[] { Y, X }, new char[] { U, IU }, new char[] { IU, IU }, new char[] { A, IA }, new char[] { IA, IA }, new char[] { O, IU }, new char[] { E, IU }, new char[] { I, M, I }, new char[] { Y, M, I }, new char[] { E, G, O }, new char[] { O, G, O }, new char[] { E, M, U }, new char[] { O, M, U } };

        private static char[][] participleEndings1 = new char[][] { new char[] { SHCH }, new char[] { E, M }, new char[] { N, N }, new char[] { V, SH }, new char[] { IU, SHCH } };

        private static char[][] participleEndings2 = new char[][] { new char[] { I, V, SH }, new char[] { Y, V, SH }, new char[] { U, IU, SHCH } };

        private static char[][] participle1Predessors = new char[][] { new char[] { A }, new char[] { IA } };

        private static char[][] reflexiveEndings = new char[][] { new char[] { S, IA }, new char[] { S, SOFT } };

        private static char[][] verbEndings1 = new char[][] { new char[] { I_ }, new char[] { L }, new char[] { N }, new char[] { L, O }, new char[] { N, O }, new char[] { E, T }, new char[] { IU, T }, new char[] { L, A }, new char[] { N, A }, new char[] { L, I }, new char[] { E, M }, new char[] { N, Y }, new char[] { E, T, E }, new char[] { I_, T, E }, new char[] { T, SOFT }, new char[] { E, SH, SOFT }, new char[] { N, N, O } };

        private static char[][] verbEndings2 = new char[][] { new char[] { IU }, new char[] { U, IU }, new char[] { E, N }, new char[] { E, I_ }, new char[] { IA, T }, new char[] { U, I_ }, new char[] { I, L }, new char[] { Y, L }, new char[] { I, M }, new char[] { Y, M }, new char[] { I, T }, new char[] { Y, T }, new char[] { I, L, A }, new char[] { Y, L, A }, new char[] { E, N, A }, new char[] { I, T, E }, new char[] { I, L, I }, new char[] { Y, L, I }, new char[] { I, L, O }, new char[] { Y, L, O }, new char[] { E, N, O }, new char[] { U, E, T }, new char[] { U, IU, T }, new char[] { E, N, Y }, new char[] { I, T, SOFT }, new char[] { Y, T, SOFT }, new char[] { I, SH, SOFT }, new char[] { E, I_, T, E }, new char[] { U, I_, T, E } };

        private static char[][] verb1Predessors = new char[][] { new char[] { A }, new char[] { IA } };

        private static char[][] nounEndings = new char[][] { new char[] { A }, new char[] { U }, new char[] { I_ }, new char[] { O }, new char[] { U }, new char[] { E }, new char[] { Y }, new char[] { I }, new char[] { SOFT }, new char[] { IA }, new char[] { E, V }, new char[] { O, V }, new char[] { I, E }, new char[] { SOFT, E }, new char[] { IA, X }, new char[] { I, IU }, new char[] { E, I }, new char[] { I, I }, new char[] { E, I_ }, new char[] { O, I_ }, new char[] { E, M }, new char[] { A, M }, new char[] { O, M }, new char[] { A, X }, new char[] { SOFT, IU }, new char[] { I, IA }, new char[] { SOFT, IA }, new char[] { I, I_ }, new char[] { IA, M }, new char[] { IA, M, I }, new char[] { A, M, I }, new char[] { I, E, I_ }, new char[] { I, IA, M }, new char[] { I, E, M }, new char[] { I, IA, X }, new char[] { I, IA, M, I } };

        private static char[][] superlativeEndings = new char[][] { new char[] { E, I_, SH }, new char[] { E, I_, SH, E } };

        private static char[][] derivationalEndings = new char[][] { new char[] { O, S, T }, new char[] { O, S, T, SOFT } };

        /// <summary> RussianStemmer constructor comment.</summary>
        public RussianStemmer()
            : base()
        {
        }

        /// <summary> RussianStemmer constructor comment.</summary>
        public RussianStemmer(char[] charset)
            : base()
        {
            this.charset = charset;
        }

        /// <summary> Adjectival ending is an adjective ending,
        /// optionally preceded by participle ending.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool Adjectival(System.Text.StringBuilder stemmingZone)
        {
            // look for adjective ending in a stemming zone
            if (!FindAndRemoveEnding(stemmingZone, adjectiveEndings))
                return false;
            // if adjective ending was found, try for participle ending
            bool r = FindAndRemoveEnding(stemmingZone, participleEndings1, participle1Predessors) || FindAndRemoveEnding(stemmingZone, participleEndings2);
            return true;
        }

        /// <summary> Derivational endings
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool Derivational(System.Text.StringBuilder stemmingZone)
        {
            int endingLength = FindEnding(stemmingZone, derivationalEndings);
            if (endingLength == 0)
                // no derivational ending found
                return false;
            else
            {
                // Ensure that the ending locates in R2
                if (R2 - RV <= stemmingZone.Length - endingLength)
                {
                    stemmingZone.Length -= endingLength;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary> Finds ending among given ending class and returns the length of ending found(0, if not found).
        /// Creation date: (17/03/2002 8:18:34 PM)
        /// </summary>
        private int FindEnding(System.Text.StringBuilder stemmingZone, int startIndex, char[][] theEndingClass)
        {
            bool match = false;
            for (int i = theEndingClass.Length - 1; i >= 0; i--)
            {
                char[] theEnding = theEndingClass[i];
                // check if the ending is bigger than stemming zone
                if (startIndex < theEnding.Length - 1)
                {
                    match = false;
                    continue;
                }
                match = true;
                int stemmingIndex = startIndex;
                for (int j = theEnding.Length - 1; j >= 0; j--)
                {
                    if (stemmingZone[stemmingIndex--] != charset[theEnding[j]])
                    {
                        match = false;
                        break;
                    }
                }
                // check if ending was found
                if (match)
                {
                    return theEndingClass[i].Length; // cut ending
                }
            }
            return 0;
        }

        private int FindEnding(System.Text.StringBuilder stemmingZone, char[][] theEndingClass)
        {
            return FindEnding(stemmingZone, stemmingZone.Length - 1, theEndingClass);
        }

        /// <summary> Finds the ending among the given class of endings and removes it from stemming zone.
        /// Creation date: (17/03/2002 8:18:34 PM)
        /// </summary>
        private bool FindAndRemoveEnding(System.Text.StringBuilder stemmingZone, char[][] theEndingClass)
        {
            int endingLength = FindEnding(stemmingZone, theEndingClass);
            if (endingLength == 0)
                // not found
                return false;
            else
            {
                stemmingZone.Length -= endingLength;
                // cut the ending found
                return true;
            }
        }

        /// <summary> Finds the ending among the given class of endings, then checks if this ending was
        /// preceded by any of given predessors, and if so, removes it from stemming zone.
        /// Creation date: (17/03/2002 8:18:34 PM)
        /// </summary>
        private bool FindAndRemoveEnding(System.Text.StringBuilder stemmingZone, char[][] theEndingClass, char[][] thePredessors)
        {
            int endingLength = FindEnding(stemmingZone, theEndingClass);
            if (endingLength == 0)
                // not found
                return false;
            else
            {
                int predessorLength = FindEnding(stemmingZone, stemmingZone.Length - endingLength - 1, thePredessors);
                if (predessorLength == 0)
                    return false;
                else
                {
                    stemmingZone.Length -= endingLength;
                    // cut the ending found
                    return true;
                }
            }
        }

        /// <summary> Marks positions of RV, R1 and R2 in a given word.
        /// Creation date: (16/03/2002 3:40:11 PM)
        /// </summary>
        private void MarkPositions(System.String word)
        {
            RV = 0;
            R1 = 0;
            R2 = 0;
            int i = 0;
            // find RV
            while (word.Length > i && !IsVowel(word[i]))
            {
                i++;
            }
            if (word.Length - 1 < ++i)
                return; // RV zone is empty
            RV = i;
            // find R1
            while (word.Length > i && IsVowel(word[i]))
            {
                i++;
            }
            if (word.Length - 1 < ++i)
                return; // R1 zone is empty
            R1 = i;
            // find R2
            while (word.Length > i && !IsVowel(word[i]))
            {
                i++;
            }
            if (word.Length - 1 < ++i)
                return; // R2 zone is empty
            while (word.Length > i && IsVowel(word[i]))
            {
                i++;
            }
            if (word.Length - 1 < ++i)
                return; // R2 zone is empty
            R2 = i;
        }

        /// <summary> Checks if character is a vowel..
        /// Creation date: (16/03/2002 10:47:03 PM)
        /// </summary>
        /// <returns> boolean
        /// </returns>
        /// <param name="letter">char
        /// </param>
        private bool IsVowel(char letter)
        {
            for (int i = 0; i < vowels.Length; i++)
            {
                if (letter == charset[vowels[i]])
                    return true;
            }
            return false;
        }

        /// <summary> Noun endings.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool Noun(System.Text.StringBuilder stemmingZone)
        {
            return FindAndRemoveEnding(stemmingZone, nounEndings);
        }

        /// <summary> Perfective gerund endings.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool PerfectiveGerund(System.Text.StringBuilder stemmingZone)
        {
            return FindAndRemoveEnding(stemmingZone, perfectiveGerundEndings1, perfectiveGerund1Predessors) || FindAndRemoveEnding(stemmingZone, perfectiveGerundEndings2);
        }

        /// <summary> Reflexive endings.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool Reflexive(System.Text.StringBuilder stemmingZone)
        {
            return FindAndRemoveEnding(stemmingZone, reflexiveEndings);
        }

        /// <summary> Insert the method's description here.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool RemoveI(System.Text.StringBuilder stemmingZone)
        {
            if (stemmingZone.Length > 0 && stemmingZone[stemmingZone.Length - 1] == charset[I])
            {
                stemmingZone.Length -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Insert the method's description here.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool RemoveSoft(System.Text.StringBuilder stemmingZone)
        {
            if (stemmingZone.Length > 0 && stemmingZone[stemmingZone.Length - 1] == charset[SOFT])
            {
                stemmingZone.Length -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Insert the method's description here.
        /// Creation date: (16/03/2002 10:58:42 PM)
        /// </summary>
        /// <param name="newCharset">char[]
        /// </param>
        public virtual void SetCharset(char[] newCharset)
        {
            charset = newCharset;
        }

        /// <summary> Set ending definition as in Russian stemming algorithm.
        /// Creation date: (16/03/2002 11:16:36 PM)
        /// </summary>
        private void SetEndings()
        {
            vowels = new char[] { A, E, I, O, U, Y, AE, IU, IA };

            perfectiveGerundEndings1 = new char[][] { new char[] { V }, new char[] { V, SH, I }, new char[] { V, SH, I, S, SOFT } };

            perfectiveGerund1Predessors = new char[][] { new char[] { A }, new char[] { IA } };

            perfectiveGerundEndings2 = new char[][] { new char[] { I, V }, new char[] { Y, V }, new char[] { I, V, SH, I }, new char[] { Y, V, SH, I }, new char[] { I, V, SH, I, S, SOFT }, new char[] { Y, V, SH, I, S, SOFT } };

            adjectiveEndings = new char[][] { new char[] { E, E }, new char[] { I, E }, new char[] { Y, E }, new char[] { O, E }, new char[] { E, I_ }, new char[] { I, I_ }, new char[] { Y, I_ }, new char[] { O, I_ }, new char[] { E, M }, new char[] { I, M }, new char[] { Y, M }, new char[] { O, M }, new char[] { I, X }, new char[] { Y, X }, new char[] { U, IU }, new char[] { IU, IU }, new char[] { A, IA }, new char[] { IA, IA }, new char[] { O, IU }, new char[] { E, IU }, new char[] { I, M, I }, new char[] { Y, M, I }, new char[] { E, G, O }, new char[] { O, G, O }, new char[] { E, M, U }, new char[] { O, M, U } };

            participleEndings1 = new char[][] { new char[] { SHCH }, new char[] { E, M }, new char[] { N, N }, new char[] { V, SH }, new char[] { IU, SHCH } };

            participleEndings2 = new char[][] { new char[] { I, V, SH }, new char[] { Y, V, SH }, new char[] { U, IU, SHCH } };

            participle1Predessors = new char[][] { new char[] { A }, new char[] { IA } };

            reflexiveEndings = new char[][] { new char[] { S, IA }, new char[] { S, SOFT } };

            verbEndings1 = new char[][] { new char[] { I_ }, new char[] { L }, new char[] { N }, new char[] { L, O }, new char[] { N, O }, new char[] { E, T }, new char[] { IU, T }, new char[] { L, A }, new char[] { N, A }, new char[] { L, I }, new char[] { E, M }, new char[] { N, Y }, new char[] { E, T, E }, new char[] { I_, T, E }, new char[] { T, SOFT }, new char[] { E, SH, SOFT }, new char[] { N, N, O } };

            verbEndings2 = new char[][] { new char[] { IU }, new char[] { U, IU }, new char[] { E, N }, new char[] { E, I_ }, new char[] { IA, T }, new char[] { U, I_ }, new char[] { I, L }, new char[] { Y, L }, new char[] { I, M }, new char[] { Y, M }, new char[] { I, T }, new char[] { Y, T }, new char[] { I, L, A }, new char[] { Y, L, A }, new char[] { E, N, A }, new char[] { I, T, E }, new char[] { I, L, I }, new char[] { Y, L, I }, new char[] { I, L, O }, new char[] { Y, L, O }, new char[] { E, N, O }, new char[] { U, E, T }, new char[] { U, IU, T }, new char[] { E, N, Y }, new char[] { I, T, SOFT }, new char[] { Y, T, SOFT }, new char[] { I, SH, SOFT }, new char[] { E, I_, T, E }, new char[] { U, I_, T, E } };

            verb1Predessors = new char[][] { new char[] { A }, new char[] { IA } };

            nounEndings = new char[][] { new char[] { A }, new char[] { IU }, new char[] { I_ }, new char[] { O }, new char[] { U }, new char[] { E }, new char[] { Y }, new char[] { I }, new char[] { SOFT }, new char[] { IA }, new char[] { E, V }, new char[] { O, V }, new char[] { I, E }, new char[] { SOFT, E }, new char[] { IA, X }, new char[] { I, IU }, new char[] { E, I }, new char[] { I, I }, new char[] { E, I_ }, new char[] { O, I_ }, new char[] { E, M }, new char[] { A, M }, new char[] { O, M }, new char[] { A, X }, new char[] { SOFT, IU }, new char[] { I, IA }, new char[] { SOFT, IA }, new char[] { I, I_ }, new char[] { IA, M }, new char[] { IA, M, I }, new char[] { A, M, I }, new char[] { I, E, I_ }, new char[] { I, IA, M }, new char[] { I, E, M }, new char[] { I, IA, X }, new char[] { I, IA, M, I } };

            superlativeEndings = new char[][] { new char[] { E, I_, SH }, new char[] { E, I_, SH, E } };

            derivationalEndings = new char[][] { new char[] { O, S, T }, new char[] { O, S, T, SOFT } };
        }

        /// <summary> Finds the stem for given Russian word.
        /// Creation date: (16/03/2002 3:36:48 PM)
        /// </summary>
        /// <returns> java.lang.String
        /// </returns>
        /// <param name="input">java.lang.String
        /// </param>
        public virtual System.String Stem(System.String input)
        {
            MarkPositions(input);
            if (RV == 0)
                return input; //RV wasn't detected, nothing to stem
            System.Text.StringBuilder stemmingZone = new System.Text.StringBuilder(input.Substring(RV));
            // stemming goes on in RV
            // Step 1

            if (!PerfectiveGerund(stemmingZone))
            {
                Reflexive(stemmingZone);
                bool r = Adjectival(stemmingZone) || Verb(stemmingZone) || Noun(stemmingZone);
            }
            // Step 2
            RemoveI(stemmingZone);
            // Step 3
            Derivational(stemmingZone);
            // Step 4
            Superlative(stemmingZone);
            UndoubleN(stemmingZone);
            RemoveSoft(stemmingZone);
            // return result
            return input.Substring(0, (RV) - (0)) + stemmingZone.ToString();
        }

        /// <summary> Superlative endings.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool Superlative(System.Text.StringBuilder stemmingZone)
        {
            return FindAndRemoveEnding(stemmingZone, superlativeEndings);
        }

        /// <summary> Undoubles N.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool UndoubleN(System.Text.StringBuilder stemmingZone)
        {
            char[][] doubleN = new char[][] { new char[] { N, N } };
            if (FindEnding(stemmingZone, doubleN) != 0)
            {
                stemmingZone.Length -= 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Verb endings.
        /// Creation date: (17/03/2002 12:14:58 AM)
        /// </summary>
        /// <param name="stemmingZone">java.lang.StringBuffer
        /// </param>
        private bool Verb(System.Text.StringBuilder stemmingZone)
        {
            return FindAndRemoveEnding(stemmingZone, verbEndings1, verb1Predessors) || FindAndRemoveEnding(stemmingZone, verbEndings2);
        }

        /// <summary> Static method for stemming with different charsets</summary>
        public static System.String Stem(System.String theWord, char[] charset)
        {
            RussianStemmer stemmer = new RussianStemmer();
            stemmer.SetCharset(charset);
            return stemmer.Stem(theWord);
        }
    }
}
