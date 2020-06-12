using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Reststops.Presentation.Web
{

    public class MapboxStaticMap
    {
        private StringBuilder _url;
        private string _mapboxToken;
        private int _width;
        private int _height;
        private string _style = "streets-v11";
        private List<Marker> _markers;
        private List<Path> _paths;

        #region Public Inner Classes

        public enum MarkerType
        {
            PIN_L,
            PIN_S
        }

        public class Marker
        {
            #region Private Members

            private string _label;
            private string _colorCode;
            private double _latitude;
            private double _longitude;

            #endregion

            #region Public Properties

            public MarkerType Type { get; set; }

            public string Label
            {
                get
                {
                    return _label;
                }
                set
                {
                    var validationRegex = new Regex("[a-zA-Z0-9]{0,2}");
                    if (!validationRegex.IsMatch(value))
                    {
                        throw new ArgumentException("Options are an alphanumeric label a through z, 0 through 99");
                    }

                    _label = value;
                }
            }

            public string ColorCode
            {
                get
                {
                    return _colorCode;
                }
                set
                {
                    var validationRegex = new Regex("^(?:[0-9a-fA-F]{3}){1,2}$");
                    if (!validationRegex.IsMatch(value))
                    {
                        throw new ArgumentException($"{value} is not a 3- or 6-digit hexadecimal color code");
                    }

                    _colorCode = value;
                }
            }

            public double Latitude
            {
                get
                {
                    return _latitude;
                }
                set
                {

                    if (value < -90 || value > 90)
                    {
                        throw new ArgumentOutOfRangeException("A valid latitude value ranges between -90 and 90");
                    }

                    _latitude = value;
                }
            }

            public double Longitude
            {
                get
                {
                    return _longitude;
                }
                set
                {

                    if (value < -180 || value > 180)
                    {
                        throw new ArgumentOutOfRangeException("A valid longitude value ranges between -90 and 90");
                    }

                    _longitude = value;
                }
            }

            #endregion

            #region Public Methods

            public override string ToString()
            {
                var name = "";

                switch (Type)
                {
                    case MarkerType.PIN_L:
                        name = "pin-l";
                        break;
                    case MarkerType.PIN_S:
                        name = "pin-s";
                        break;
                }

                // e.g. pin-l-1+000(-122.42816,37.75965)
                return $"{name}-{Label}+{ColorCode}({Longitude},{Latitude})";
            }

            #endregion
        }

        public class Path
        {
            #region Private Members

            private double _strokeOpacity;
            private string _strokeColor;
            private string _fillColor;
            private double _fillOpacity;

            #endregion

            #region Public Properties

            public ushort StrokeWidth { get; set; }

            public string StrokeColor
            {
                get
                {
                    return _strokeColor;
                }
                set
                {
                    var validationRegex = new Regex("^(?:[0-9a-fA-F]{3}){1,2}$");
                    if (!validationRegex.IsMatch(value))
                    {
                        throw new ArgumentException($"{value} is not a 3- or 6-digit hexadecimal color code");
                    }

                    _strokeColor = value;
                }
            }

            public string FillColor
            {
                get
                {
                    return _fillColor;
                }
                set
                {
                    var validationRegex = new Regex("^(?:[0-9a-fA-F]{3}){1,2}$");
                    if (!validationRegex.IsMatch(value))
                    {
                        throw new ArgumentException($"{value} is not a 3- or 6-digit hexadecimal color code");
                    }

                    _fillColor = value;
                }
            }

            public double StrokeOpacity
            {
                get
                {
                    return _strokeOpacity;
                }
                set
                {
                    if(value < 0 || value > 1)
                    {
                        throw new ArgumentException("StrokeOpacity must be a value between 0 and 1");
                    }

                    _strokeOpacity = value;
                }
            }

            public double FillOpacity
            {
                get
                {
                    return _fillOpacity;
                }
                set
                {
                    if (value < 0 || value > 1)
                    {
                        throw new ArgumentException("FillOpacity must be a value between 0 and 1");
                    }

                    _fillOpacity = value;
                }
            }

            public string Polyline { get; set; }

            #endregion

            #region Public Methods

            public override string ToString()
            {
                return $"path-{StrokeWidth}+{StrokeColor}-{StrokeOpacity}+{FillColor}-{FillOpacity}({Polyline})";
            }

            #endregion
        }

        #endregion

        public MapboxStaticMap(string mapboxToken)
        {
            _mapboxToken = mapboxToken ?? throw new ArgumentNullException(nameof(mapboxToken));
            _url = new StringBuilder($"https://api.mapbox.com/styles/v1/mapbox/");

            _markers = new List<Marker>();
            _paths = new List<Path>();
        }

        #region Public Methods

        public MapboxStaticMap SetStyle(string style)
        {
            if(!string.IsNullOrWhiteSpace(style))
            {
                _style = style;
            }

            return this;
        }

        public MapboxStaticMap SetDimensions(int width, int height)
        {
            _width = Math.Max(Math.Min(1280, width), 1);
            _height = Math.Max(Math.Min(1280, height), 1);

            return this;
        }

        public MapboxStaticMap AddMarker(Marker marker)
        {
            _markers.Add(marker);
            return this;
        }

        public MapboxStaticMap AddPath(Path path)
        {
            _paths.Add(path);
            return this;
        }

        public override string ToString()
        {
            // add style
            _url.Append($"streets-v11/static/");

            // add markers
            _url.Append(string.Join(',',
                _markers.Select(m => m.ToString())
            ));

            if (_markers.Any() && _paths.Any())
            {
                _url.Append(",");
            }

            // add paths
            _url.Append(string.Join(',',
                _paths.Select(p => p.ToString())
            ));

            if (_markers.Any() || _paths.Any())
            {
                _url.Append("/");
            }

            // add center/zoom
            _url.Append("auto/");

            // add dimensions
            _url.Append($"{_width}x{_height}");

            // token
            _url.Append($"?access_token={_mapboxToken}");

            return _url.ToString();
        }

        #endregion
    }
}
