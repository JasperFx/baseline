using System;
using System.Collections.Generic;

namespace Baseline.Conversion
{
    public class Conversions
    {
        private readonly LightweightCache<Type, Func<string, object>> _convertors;
        private readonly IList<IConversionProvider> _providers = new List<IConversionProvider>();


        public Conversions()
        {
            _convertors =
                new LightweightCache<Type, Func<string, object>>(
                    type =>
                    {
                        return
                            _providers.UnionWith(new EnumerationConversion(), new NullableConvertor(this),
                                new ArrayConversion(this), new StringConverterProvider())
                                .FirstValue(x => x.ConverterFor(type));
                    });

            RegisterConversion(bool.Parse);
            RegisterConversion(byte.Parse);
            RegisterConversion(sbyte.Parse);
            RegisterConversion(char.Parse);
            RegisterConversion(decimal.Parse);
            RegisterConversion(double.Parse);
            RegisterConversion(float.Parse);
            RegisterConversion(short.Parse);
            RegisterConversion(int.Parse);
            RegisterConversion(long.Parse);
            RegisterConversion(ushort.Parse);
            RegisterConversion(uint.Parse);
            RegisterConversion(ulong.Parse);
            RegisterConversion(DateTimeConverter.GetDateTime);

            RegisterConversion(x =>
            {
                if (x == "EMPTY") return string.Empty;

                return x;
            });
        }


        public void RegisterConversionProvider<T>() where T : IConversionProvider, new()
        {
            _providers.Add(new T());
        }

        public void RegisterConversion<T>(Func<string, T> convertor)
        {
            _convertors[typeof (T)] = x => convertor(x);
        }

        public Func<string, object> FindConverter(Type type)
        {
            return _convertors[type];
        }

        public object Convert(Type type, string raw)
        {
            return _convertors[type](raw);
        }
    }
}