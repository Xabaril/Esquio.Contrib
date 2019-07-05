using Esquio.Abstractions;
using Esquio.Model;

namespace FunctionalTests.Seedwork
{
    public static class Build
    {
        public static FeatureBuilder Feature(string name)
        {
            return new FeatureBuilder(name);
        }

        public static ToggleBuilder Toggle<TToggle>()
            where TToggle : IToggle
        {
            return new ToggleBuilder(typeof(TToggle).FullName);
        }
    }

    public class FeatureBuilder
    {
        private readonly Feature feature;

        public FeatureBuilder(string name)
        {
            feature = new Feature(name);
        }

        public FeatureBuilder AddOne(Toggle toggle)
        {
            feature.AddToggle(toggle);
            return this;
        }

        public FeatureBuilder Enabled()
        {
            feature.Enabled();
            return this;
        }
        public FeatureBuilder Disabled()
        {
            feature.Disabled();
            return this;
        }


        public Feature Build()
        {
            return feature;
        }
    }

    public class ToggleBuilder
    {
        private Toggle _toggle;

        public ToggleBuilder(string type)
        {
            _toggle = new Toggle(type);
        }

        public ToggleBuilder AddOneParameter(string name, object value)
        {
            _toggle.AddParameters(new Parameter[]
            {
                new Parameter(name, value)
            });

            return this;
        }

        public Toggle Build()
        {
            return _toggle;
        }
    }
}
