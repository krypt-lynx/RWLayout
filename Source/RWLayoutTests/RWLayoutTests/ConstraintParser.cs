using Cassowary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayoutTests.ConstraintParser
{
    [TestClass]
    public class ConstraintParser
    {
        struct TestSuccessEntry
        {
            public TestSuccessEntry(string constraint, string[] terms, Cl.Operator @operator)
            {
                this.Constraint = constraint;
                this.ExpectedTerms = terms;
                this.Operator = @operator;
            }

            public string Constraint;
            public string[] ExpectedTerms;
            public Cl.Operator Operator;
        }

        [TestMethod]
        public void ParseSuccess()
        {
            var parser = new RWLayout.alpha2.ConstraintParser();

            var tests = new TestSuccessEntry[] {
                new TestSuccessEntry(
                    "dlayout.height == dlayout.intrinsicHeight",
                    new string[]{ "1×dlayout.height", "-1×dlayout.intrinsicHeight" },
                    Cl.Operator.EqualTo
                    ),
                new TestSuccessEntry(
                    "noexamples.top == aaass.bottom+20",
                    new string[]{ "1×noexamples.top", "-1×aaass.bottom", "-20" },
                    Cl.Operator.EqualTo
                    ),
                new TestSuccessEntry(
                    "a.x/10 + b.y*10 >= test.tt - test.dd - 2*test.ac + 0*test.d",
                    new string[]{ "0.1×a.x", "10×b.y", "-1×test.tt", "1×test.dd", "2×test.ac" },
                    Cl.Operator.GreaterThanOrEqualTo
                    ),
                new TestSuccessEntry(
                    "a.x + 10 + 20 + 30 >= 0",
                    new string[]{ "1×a.x", "10", "20", "30" },
                    Cl.Operator.GreaterThanOrEqualTo
                    ),
                new TestSuccessEntry(
                    "0<=55*a.bb",
                    new string[]{ "-55×a.bb" },
                    Cl.Operator.LessThanOrEqualTo
                    ),             
                new TestSuccessEntry(
                    "a.x + ==",
                    new string[]{ "1×a.x" },
                    Cl.Operator.EqualTo
                    ),
                new TestSuccessEntry(
                    "a.x ==",
                    new string[]{ "1×a.x" },
                    Cl.Operator.EqualTo
                    ),
                new TestSuccessEntry(
                    "== a.x",
                    new string[]{ "-1×a.x" },
                    Cl.Operator.EqualTo
                    ),
            };


            foreach (var test in tests)
            {
                var result = parser.Parse(test.Constraint);
                Assert.IsTrue(result.Terms.Select(x => x.ToString()).SequenceEqual(test.ExpectedTerms), test.Constraint);
                Assert.AreEqual(test.Operator, result.Operator);
            }
        }

        struct TestThrowsEmtry
        {
            public TestThrowsEmtry(string constraint, Type Exception)
            {
                this.Constraint = constraint;
                this.ExceptionType = Exception;
            }

            public string Constraint;
            public Type ExceptionType;
        }

        [TestMethod]
        public void ParseThrows()
        {
            var parser = new RWLayout.alpha2.ConstraintParser();

            var tests = new TestThrowsEmtry[] {
                new TestThrowsEmtry(
                    "test", typeof(SyntaxErrorException)
                    ),
                new TestThrowsEmtry(
                    "test.ab", typeof(SyntaxErrorException)
                    ),
                new TestThrowsEmtry(
                    "2*test", typeof(SyntaxErrorException)
                    ),
                new TestThrowsEmtry(
                    "2*test.ab*2", typeof(SyntaxErrorException)
                    ),
                new TestThrowsEmtry(
                    "a.x = b.y", typeof(SyntaxErrorException)
                    ),
                new TestThrowsEmtry(
                    "a.x/b.y == 1", typeof(SyntaxErrorException)
                    ),                           
                new TestThrowsEmtry(
                    "2/b.y == 1", typeof(SemanticErrorException)
                    ),
                new TestThrowsEmtry(
                    "a.x/0 == 1", typeof(SemanticErrorException)
                    ),
                new TestThrowsEmtry(
                    "<=", typeof(SemanticErrorException)
                    ),
            };

            foreach (var test in tests)
            {
                try
                {
                    var result = parser.Parse(test.Constraint);

                } 
                catch (Exception e)
                {
                    Assert.AreEqual(test.ExceptionType, e.GetType(), test.Constraint);
                    continue;
                }

                Assert.Fail($"No exception was thrown for constraint \"{test.Constraint}\"");

            }
        }
    }
}
