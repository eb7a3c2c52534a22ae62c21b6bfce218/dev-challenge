using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand;
using Xunit;

namespace Wool.DevChallenge.Api.Tests
{
    [Trait("Category", "Lean")]
    public class LocalTrolleyCalculationsServiceTests
    {
        [Theory]
        [ClassData(typeof(TestTrolleys))]
        public async Task GetTrolleyCalculations_DataTests(CalculateTrolleyTotalCommand.RequestTrolley trolley, decimal value)
        {
            var mockLogger = new Mock<ILogger<LocalTrolleyCalculationService>>();

            var svc = new LocalTrolleyCalculationService(mockLogger.Object);
            var data = await svc.GetTrolleyCalculations(trolley, default(CancellationToken));
            data.ShouldBe(value);
        }

        private class TestTrolleys : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[]
                {
                    new CalculateTrolleyTotalCommand.RequestTrolley
                    {
                        Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct
                                {Name = "Test Product A", Price = 9.0M}
                        },
                        Specials = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial
                            {
                                Quantities =
                                    new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                                    {
                                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                                            {Name = "Test Product A", Quantity = 2}
                                    },
                                Total = 9
                            }
                        },
                        Quantities = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                            {
                                Name = "Test Product A", Quantity = 2
                            }
                        }
                    },
                    9
                };
                yield return new object[]
                {
                    new CalculateTrolleyTotalCommand.RequestTrolley
                    {
                        Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct
                                {Name = "Test Product A", Price = 9.0M}
                        },
                        Specials = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial
                            {
                                Quantities =
                                    new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                                    {
                                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                                            {Name = "Test Product A", Quantity = 2}
                                    },
                                Total = 18
                            }
                        },
                        Quantities = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                            {
                                Name = "Test Product A", Quantity = 2
                            }
                        }
                    },
                    18
                };
                yield return new object[]
                {
                    new CalculateTrolleyTotalCommand.RequestTrolley
                    {
                        Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct
                                {Name = "Test Product A", Price = 10.0M}
                        },
                        Specials = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial
                            {
                                Quantities =
                                    new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                                    {
                                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                                            {Name = "Test Product A", Quantity = 2},
                                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                                            {Name = "Test Product B", Quantity = 3}

                                    },
                                Total = 9
                            }
                        },
                        Quantities = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                            {
                                Name = "Test Product A", Quantity = 11
                            }
                        }
                    },
                    55
                };
                yield return new object[]
                {
                    new CalculateTrolleyTotalCommand.RequestTrolley
                    {
                        Products = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProduct
                                {Name = "Test Product A", Price = 10.0M}
                        },
                        Specials = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleySpecial
                            {
                                Quantities =
                                    new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                                    {
                                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                                            {Name = "Test Product A", Quantity = 2},
                                        new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                                            {Name = "Test Product B", Quantity = 3}

                                    },
                                Total = 0
                            }
                        },
                        Quantities = new List<CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity>
                        {
                            new CalculateTrolleyTotalCommand.RequestTrolley.TrolleyProductQuantity
                            {
                                Name = "Test Product A", Quantity = 11
                            }
                        }
                    },
                    110
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }



    }
}
