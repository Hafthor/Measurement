# Measurement

A library of strongly-typed C# measurement classes — `Length`, `Mass`, `Force`, `Energy`, and about
80 more. Each quantity is its own immutable value type, so the compiler stops you mixing incompatible
units, and dimensioned arithmetic (`Speed = Length / Duration`) just works.

Requires C# 14 / .NET 10.

## Two ways to use it

Everything in the library is reachable two equivalent ways — pick whichever reads best in context, and
mix them freely (the result of one is an ordinary measurement you can feed to the other):

**1. Explicit methods** — a `FromXxx` factory and a `ToXxx` accessor for every unit:

```csharp
Length d  = Length.FromKilometers(5);
double mi = d.ToMiles();
Speed  v  = d / Duration.FromHours(2);          // dimensioned arithmetic
```

**2. Fluent interface** — name units to build a value, and again after `.To` to read one back:

```csharp
Length d  = Measure.Of(5).Kilo.Meters;
double mi = d.To.Miles;
Speed  v  = Measure.Of(10).Meters.Per.Second;
```

The fluent form composes units word-by-word (`.Kilo.Meters`, `.Meters.Per.Second`, `.Joule.Seconds`);
see [Fluent interface](#fluent-interface) and [Compositional grammar](#compositional-fluent-grammar).

## What every measurement provides

Whichever style you use, every quantity type offers the same surface:

- **Construction / read-out** — `T.FromUnit(double)` factories and `double x.ToUnit()` accessors for
  every supported unit.
- **Value semantics** — equality and hashing (`==`, `!=`, `Equals`, plus `NearlyEquals` for
  ULP-tolerant comparison) and ordering (`<`, `>`, `<=`, `>=`, `IComparable<T>`).
- **Arithmetic** — same-type `+`, `-`, and unary negation; scalar `* k` and `/ k`; same-type `/` → a
  `Ratio`; and `Abs`/`Min`/`Max`/`Clamp`/`Lerp`.
- **Cross-type operators** — dimensioned relations such as `Force = Mass * Acceleration` and
  `Energy = Force * Length` (see [Worked relations](#worked-relations)).
- **Formatting & parsing** — `ToString()`/`IFormattable` render the SI unit (`"10 m/s"`), and
  `Parse`/`TryParse` (`IParsable<T>`/`ISpanParsable<T>`) read it back.
- **Generic math** — implements `IMeasurement<T>` and the `System.Numerics` operator interfaces, so
  you can write `T Sum<T>(IEnumerable<T> xs) where T : IMeasurement<T>`.

`ToString()` renders each value in its **fundamental SI unit** (`Force.FromNewtons(6)` → `"6 N"`). Two
quantities show a friendlier form: `Mass` prints grams (`"2000 g"`), and the dimensionless `Quantity`
and `Ratio` print a bare number.

Same-type addition is well-defined for **`Temperature`** as well — its canonical scale is kelvin, an
absolute (true-zero) scale — though a sum read back on an offset scale looks shifted
(`0 °C + 0 °C` = 273.15 °C).

## Fluent interface

The fluent interface builds and reads values by naming units, with all 24 SI prefixes
(`Quetta`…`Quecto`) available on every quantity so you never spell out `FromKilo…`/`FromMilli…`.

**Construction** — start with `Measure.Of(value)` and name a unit. Each quantity has a direct hook
for its **base SI unit** and its **non-SI units**; the SI-prefixed decades (kilo, milli, …) come from
the prefix chain rather than separate `Kilometers`/`Milligrams` members, and prefixes stack:

```csharp
Mass    m = Measure.Of(5).Kilo.Grams;       // 5 kg  (mass prefixes attach to the gram)
Length  d = Measure.Of(3).Kilo.Meters;      // 3 km  (not .Kilometers — use the chain)
Length  y = Measure.Of(1).Miles;            // non-SI unit → direct hook
Energy  e = Measure.Of(2).Mega.Joules;      // 2 MJ
Length  x = Measure.Of(1).Mega.Mega.Meters; // prefixes stack → 1e12 m
```

**Read-out** — `.To` then a unit (optionally prefixed) returns a `double`:

```csharp
double lb = Measure.Of(5).Kilo.Grams.To.Pounds;   // 5 kg expressed in pounds
double km = someLength.To.Kilo.Meters;            // metres → km
double ms = duration.To.Milli.Seconds;            // seconds → ms
double f  = temperature.To.Fahrenheit;            // non-SI reader
```

**Opt-in `double` sugar** — to write `5.0.Kilo.Meters` directly on a number, add the import. It lives
in its own namespace so it never affects `double` unless you ask for it:

```csharp
using com.hafthor.Measurement.Fluent;   // per file, or in a GlobalUsings.cs

Mass m = 5.0.Kilo.Grams;                 // only compiles where this using is present
```

A few naming rules to know:

- **Squared/cubed metric units** are spelled with the [`.Square`/`.Cubic` grammar](#compositional-fluent-grammar)
  (`.Square.Centi.Meters`), because a lone prefix would give the wrong factor (1 cm² = 1e-4 m², not
  1e-2). Bare hooks like `.SquareCentimeters` also exist.
- **SI-prefixed unit names** aren't separate hooks — reach `Kilomoles`, `Milliamperehours`, etc.
  through the chain (`Measure.Of(1).Kilo.Moles`). The explicit `FromKilomoles`/`ToKilomoles` factories
  are always available.
- **Names shared by two quantities** are disambiguated by naming the quantity. A product spelling like
  `JouleSeconds` (shared by `Action`/`AngularMomentum`) uses the [compositional walk](#compositional-fluent-grammar):
  `Measure.Of(1).Joule.Seconds` is an `Action`, and `.Joule.Seconds.AngularMomentum` names the other
  reading. A quotient spelling like `RevolutionsPerMinute` (shared by `Frequency`/`AngularVelocity`)
  uses a trailing selector: `Measure.Of(3).RevolutionsPerMinute.Frequency`.

## Compositional fluent grammar

The prefix hooks above name a **whole** unit (`.Kilo.Meters`). On top of that, compound units compose
**word by word** — quotients via `.Per`, products by naming each factor, and areal/cubic scaling via
`.Square`/`.Cubic` — so you rarely need a dedicated hook for a compound unit. Every intermediate is a
real value (implicitly convertible to its measurement), and each slot accepts **any dimensionally
compatible unit**, not just an exactly-spelled one.

### `.Per` — quotient walk

Attach `.Per` to any numerator unit to divide by a denominator; chain it for repeated division. Each
denominator slot accepts **any (non-affine) unit of that dimension**, not just the ones that appear in
a spelled compound unit. Denominators decompose through the prefix chain just like numerators
(`.Per.Kilo.Gram`), and an areal/cubic denominator is spelled with `.Square`/`.Cubic`
(`.Per.Cubic.Centi.Meter`):

```csharp
Speed        v  = Measure.Of(10).Meters.Per.Second;             // m/s
Speed        h  = Measure.Of(90).Kilo.Meters.Per.Hour;          // any duration works: km/h
AngularVelocity r = Measure.Of(90).Degrees.Per.Minute;          // 90°/min = 1.5°/s
Acceleration a  = Measure.Of(9).Meters.Per.Second.Per.Second;   // m/s² (chained)
Acceleration a2 = Measure.Of(9).Meters.Per.Second.Squared;      // …or .Squared / .Cubed shorthand
Density      d  = Measure.Of(1).Grams.Per.Cubic.Centi.Meter;    // g/cm³ (cubic-length denominator)
SpecificHeatCapacity sh = Measure.Of(4184).Joules.Per.Kilo.Gram.Kelvin;    // J/(kg·K)
MolarHeatCapacity    mh = Measure.Of(8.314).Joules.Per.Mole.Kelvin;        // J/(mol·K)
```

The same walk works on the **read-out** side after `.To`, returning a `double`, with the same
polymorphic denominators:

```csharp
double mps = someSpeed.To.Meters.Per.Second;
double mph = someSpeed.To.Meters.Per.Hour;                 // read in any duration
double shk = someSpecificHeat.To.Joules.Per.Kilo.Gram.Kelvin;
```

### Products — name each factor

Name a product's factors in sequence; each factor multiplies the running value by one of that unit.
Because the factors compose **dimensionally**, any compatible unit fits a slot even when no single
factory spells it:

```csharp
ElectricCharge q  = Measure.Of(2).Ampere.Hours;      // A·h
Action         a  = Measure.Of(1).Joule.Minutes;     // J·min == 60 J·s (no JouleMinutes factory needed)
Length         ls = Measure.Of(1).Light.Seconds;     // .Light is c; c × time → length (a light-second)
Length         ly = Measure.Of(1).Light.Annums;      // × a Julian year → a light-year
```

A **leading SI prefix** comes from the prefix chain, and the running value is usable at any step:

```csharp
ElectricCharge mAh = Measure.Of(500).Milli.Ampere.Hours;  // mA·h
Energy         kWh = Measure.Of(3).Kilo.Watt.Hours;       // kW·h
Force          f   = Measure.Of(10).Newton;               // first token alone is already a Force
```

When a product is **dimensionally ambiguous** (Force × Length is both torque and energy; Energy ×
Duration is both action and angular momentum), the walk gives you the primary result by default, and a
trailing token names either reading explicitly:

```csharp
Energy e = Measure.Of(5).Newton.Meters.Energy;
Torque t = Measure.Of(5).Newton.Meters.Torque;
Action          a  = Measure.Of(1).Joule.Seconds;                   // the primary reading (Action)
AngularMomentum am = Measure.Of(1).Joule.Seconds.AngularMomentum;   // …or name the other reading
```

### `.Square` / `.Cubic` — areal and cubic units

`.Square`/`.Cubic` scale by an area or volume unit, decomposing the length through the prefix chain —
so you never need a dedicated `.SquareMillimeters`-style hook:

```csharp
Area   sm  = Measure.Of(4).Square.Meters;         // m²
Area   smm = Measure.Of(1).Square.Milli.Meters;   // mm² = 1e-6 m²
Volume ccm = Measure.Of(1).Cubic.Centi.Meters;    // cm³ = 1e-6 m³
```

The same modifier extends a running product, scaling it by an area or volume (Mass × Area →
MomentOfInertia, Length × Area → Volume, Pressure × Area → Force, …):

```csharp
MomentOfInertia moi = Measure.Of(1).Kilogram.Square.Meters;       // kg·m²
MomentOfInertia mi2 = Measure.Of(1).Kilogram.Square.Centi.Meters; // kg·cm² = 1e-4 kg·m²
```

All three forms also work on the opt-in `double` sugar (`2.0.Ampere.Hours`, `4.0.Square.Meters`)
wherever `using com.hafthor.Measurement.Fluent;` is in scope.

## JSON serialization

`System.Text.Json` support ships in the box. Register `MeasurementJsonConverterFactory` once and
every measurement type serializes in its **fundamental SI unit** (stable and human-readable);
dimensionless types (`Ratio`, `Quantity`) serialize as a bare number.

```csharp
var options = new JsonSerializerOptions {
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // keep unit symbols like ² · Ω literal
};
options.Converters.Add(new MeasurementJsonConverterFactory());

JsonSerializer.Serialize(Length.FromMeters(5), options);   // "5 m"
JsonSerializer.Serialize(Mass.FromKilograms(2), options);  // "2000 g"
JsonSerializer.Serialize(Quantity.FromCount(1000), options); // 1000  (bare number)
```

Reading is lenient: `"5 m"`, `"5"`, and `5` all deserialize to `Length.FromMeters(5)` (the unit
suffix is validated when present, optional otherwise). The converter is fully generic — it works for
every measurement type.

## Parsing

Every type implements `IParsable<T>` and `ISpanParsable<T>`, so `Parse`/`TryParse` are the inverse
of `ToString` (they read the value in its **fundamental SI unit**):

```csharp
Length a = Length.Parse("5 m");                       // 5 m
Length b = Length.Parse("5m");                        // space optional
Length c = Length.Parse("1000 m");                    // == Length.FromKilometers(1)
Mass   m = Mass.Parse("2000 g");                       // == Mass.FromKilograms(2)
if (Speed.TryParse("9.8 m/s", out var s)) { /* … */ } // non-throwing

// culture-aware, and usable through the generic constraint
Length d = Length.Parse("1,5 m", CultureInfo.GetCultureInfo("de-DE"));  // 1.5 m
static T Read<T>(string s) where T : IParsable<T> => T.Parse(s, CultureInfo.InvariantCulture);
```

The trailing symbol is validated when present and optional otherwise (a bare number is accepted);
a wrong unit (`"5 kg"` for `Length`) or a unit on a dimensionless type fails. Only the SI symbol is
recognised — for other units use the `FromXxx` factories (`Length.FromMiles(1)`).

### Dynamic parsing

When you don't know the type up front, `Measure.Parse`/`TryParse` inspect the **unit symbol** and
return the matching type boxed as the non-generic `IMeasurement`:

```csharp
IMeasurement m = Measure.Parse("5 m/s");   // → a Speed
m.UnitSymbol;                              // "m/s"
m.CanonicalValue;                          // stored value
if (m is Speed s) { /* strongly typed again */ }

Measure.TryParse("2000 g", out var mass);  // → a Mass (== Mass.FromKilograms(2))
```

A recognised SI unit symbol is required: a bare number is ambiguous (e.g. `Ratio` vs `Quantity`)
and is rejected.

## Worked relations

Because the operators carry dimensions, many textbook formulas fall straight out of the
type system (all verified in the test suite):

| Relation | Expressed as |
|----------|--------------|
| Mass–energy equivalence, E = mc² | `Energy e = mass * c * c;` |
| Planck–Einstein, E = hf | `Energy e = planckConstant * frequency;` |
| Newton's second law, F = ma | `Force f = mass * acceleration;` |
| Impulse–momentum, J = FΔt | `Momentum j = force * duration;` |
| Work, W = Fd | `Energy w = force * length;` |
| Power, P = Fv = VI | `Power p = force * speed;` / `voltage * current;` |
| Ohm's law, V = IR | `Voltage v = current * resistance;` |
| Wave equation, c = fλ | `Speed c = frequency * wavelength;` |
| Wave speed, v = λ/T | `Speed v = wavelength / period;` |
| Period–frequency, f = 1/T | `Frequency f = Frequency.FromPeriod(period);` |
| de Broglie, λ = h/p | `Length lambda = planckConstant / momentum;` |
| Work on a charge, W = QV | `Energy w = charge * voltage;` |
| Pressure–volume work, W = PV | `Energy w = pressure * volume;` |
| Density, ρ = m/V | `Density rho = mass / volume;` |
| Heat / calorie, Q = m·c·ΔT | `Energy q = mass * specificHeat * deltaT;` (ΔT a `Temperature` in K) |

Convenience constants mirror the unit factories: `Speed.FromSpeedOfLight(1)` (c) and
`Action.FromPlanckConstants(1)` (h).

---

## Foundational units

The dimensional SI base quantities. Everything else is derived from these.

| Class | Quantity | SI unit | Symbol |
|-------|----------|---------|--------|
| `Length` | length | metre | m |
| `Mass` | mass | gram | g |
| `Duration` | time | second | s |
| `ElectricCurrent` | electric current | ampere | A |
| `Temperature` | thermodynamic temperature | kelvin | K |
| `Quantity` | amount of substance / count | count | — |
| `LuminousIntensity` | luminous intensity | candela | cd |

> **`Mass`** displays in grams (`"2000 g"`) rather than the SI kilogram; all `From*`/`To*` methods
> are available regardless.

Three foundational quantities have usage notes worth knowing:

- **`Duration`** (not `Time`, to avoid clashing with `System.DateTime`/`TimeSpan`) represents an
  *elapsed* quantity over a huge range — from Planck time to cosmological scales. It is **not** meant
  for calendar arithmetic (adding months or years).
- **`Quantity`** models amount of substance and plain counts. Its canonical unit is the raw **count**,
  so integer counts (and whole pairs, dozens, gross) are exact; moles are `Quantity.FromMoles(n)`.
  Being dimensionless, it prints a bare number.
- **`Temperature`** is a single class (kelvin canonical); Celsius, Fahrenheit, Rankine, etc. are
  `From`/`To` methods on it rather than separate classes.

### Precision

Values are stored as a `double` (~15–16 significant digits) and always displayed in the fundamental
SI unit, so how a value is held internally never affects results or output. Typical magnitudes stay
exact; only extreme values lose integer-exactness — for example an astronomical `Length` such as a
light-year becomes approximate. `Duration` and `Temperature` keep full precision across their whole
ranges.

---

## Derived units with special SI names

The 22 SI derived units that have their own named unit. Each is a composite of
the foundational quantities (dimension shown in terms of base units).

### Geometry & mechanics

| Class | Quantity | SI unit | Symbol | Dimension |
|-------|----------|---------|--------|-----------|
| `Angle` | plane angle | radian | rad | m·m⁻¹ (dimensionless) |
| `SolidAngle` | solid angle | steradian | sr | m²·m⁻² (dimensionless) |
| `Frequency` | frequency | hertz | Hz | s⁻¹ |
| `Force` | force, weight | newton | N | kg·m·s⁻² |
| `Pressure` | pressure, stress | pascal | Pa | kg·m⁻¹·s⁻² |
| `Energy` | energy, work, heat | joule | J | kg·m²·s⁻² |
| `Power` | power, radiant flux | watt | W | kg·m²·s⁻³ |

### Electromagnetism

| Class | Quantity | SI unit | Symbol | Dimension |
|-------|----------|---------|--------|-----------|
| `ElectricCharge` | electric charge | coulomb | C | A·s |
| `Voltage` | electric potential, EMF | volt | V | kg·m²·s⁻³·A⁻¹ |
| `Capacitance` | capacitance | farad | F | kg⁻¹·m⁻²·s⁴·A² |
| `ElectricResistance` | resistance, impedance | ohm | Ω | kg·m²·s⁻³·A⁻² |
| `ElectricConductance` | conductance | siemens | S | kg⁻¹·m⁻²·s³·A² |
| `MagneticFlux` | magnetic flux | weber | Wb | kg·m²·s⁻²·A⁻¹ |
| `MagneticFluxDensity` | magnetic flux density | tesla | T | kg·s⁻²·A⁻¹ |
| `Inductance` | inductance | henry | H | kg·m²·s⁻²·A⁻² |

### Photometry

| Class | Quantity | SI unit | Symbol | Dimension |
|-------|----------|---------|--------|-----------|
| `LuminousFlux` | luminous flux | lumen | lm | cd·sr |
| `Illuminance` | illuminance | lux | lx | cd·sr·m⁻² |

### Radiation & chemistry

| Class | Quantity | SI unit | Symbol | Dimension |
|-------|----------|---------|--------|-----------|
| `Radioactivity` | activity referred to a radionuclide | becquerel | Bq | s⁻¹ |
| `AbsorbedDose` | absorbed dose (kerma) | gray | Gy | m²·s⁻² |
| `EquivalentDose` | equivalent / effective dose | sievert | Sv | m²·s⁻² |
| `CatalyticActivity` | catalytic activity | katal | kat | mol·s⁻¹ |

---

## Other composite measurements

Common derived quantities that do not have a special SI unit name but are widely
used. Each is expressed as a combination of foundational units.

### Kinematics

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Speed` | speed, velocity magnitude | m/s | Length / Duration |
| `Acceleration` | acceleration | m/s² | Speed / Duration |
| `Jerk` | jerk (jolt) | m/s³ | Acceleration / Duration |
| `AngularVelocity` | angular velocity | rad/s | Angle / Duration |
| `AngularAcceleration` | angular acceleration | rad/s² | AngularVelocity / Duration |
| `VolumetricFlowRate` | volumetric flow rate | m³/s | Volume / Duration |

### Geometry

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Area` | area | m² | Length × Length |
| `Volume` | volume | m³ | Area × Length |
| `Wavenumber` | wavenumber | m⁻¹ | 1 / Length |

### Mass-related

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Density` | mass density | g/m³ | Mass / Volume |
| `SpecificVolume` | specific volume | m³/g | Volume / Mass |
| `LinearDensity` | linear mass density | g/m | Mass / Length |
| `AreaDensity` | area (surface) density | g/m² | Mass / Area |
| `MassFlowRate` | mass flow rate | g/s | Mass / Duration |
| `Molality` | molality | mol/g | Quantity / Mass |
| `MolarMass` | molar mass | g/mol | Mass / Quantity |

### Mechanics & dynamics

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Momentum` | linear momentum, impulse | g·m/s | Mass × Speed |
| `AngularMomentum` | angular momentum | g·m²/s | MomentOfInertia × AngularVelocity |
| `Torque` | torque, moment of force | N·m | Force × Length |
| `MomentOfInertia` | moment of inertia | g·m² | Mass × Area |
| `SurfaceTension` | surface tension | N/m | Force / Length |
| `DynamicViscosity` | dynamic viscosity | Pa·s | Pressure × Duration |
| `KinematicViscosity` | kinematic viscosity | m²/s | Area / Duration |
| `Action` | action | J·s | Energy × Duration |

### Thermodynamics

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `HeatCapacity` | heat capacity, entropy | J/K | Energy / Temperature |
| `SpecificHeatCapacity` | specific heat capacity | J/(g·K) | HeatCapacity / Mass |
| `MolarHeatCapacity` | molar heat capacity | J/(mol·K) | HeatCapacity / Quantity |
| `ThermalConductivity` | thermal conductivity | W/(m·K) | Power / (Length × Temperature) |
| `ThermalResistance` | thermal resistance | K/W | Temperature / Power |
| `HeatFluxDensity` | heat flux density, irradiance | W/m² | Power / Area |

### Electromagnetism

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `ElectricFieldStrength` | electric field strength | V/m | Voltage / Length |
| `ChargeDensity` | volume charge density | C/m³ | ElectricCharge / Volume |
| `SurfaceChargeDensity` | surface charge density | C/m² | ElectricCharge / Area |
| `CurrentDensity` | current density | A/m² | ElectricCurrent / Area |
| `Permittivity` | permittivity | F/m | Capacitance / Length |
| `Permeability` | permeability | H/m | Inductance / Length |
| `MagneticFieldStrength` | magnetic field strength | A/m | ElectricCurrent / Length |
| `Resistivity` | electrical resistivity | Ω·m | ElectricResistance × Length |
| `Conductivity` | electrical conductivity | S/m | ElectricConductance / Length |
| `LinearMagneticFluxDensity` | magnetic flux per length | Wb/m | MagneticFlux / Length |
| `ElectricDipoleMoment` | electric dipole moment | C·m | ElectricCharge × Length |

### Photometry & radiation

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Luminance` | luminance | cd/m² | LuminousIntensity / Area |
| `LuminousEnergy` | luminous energy | lm·s | LuminousFlux × Duration |
| `LuminousExposure` | luminous exposure | lx·s | Illuminance × Duration |
| `Radiance` | radiance | W/(m²·sr) | Power / (Area × SolidAngle) |
| `RadiantIntensity` | radiant intensity | W/sr | Power / SolidAngle |
| `Exposure` | radiation exposure | C/g | ElectricCharge / Mass |
| `DoseRate` | absorbed dose rate | Gy/s | AbsorbedDose / Duration |

### Chemistry

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Concentration` | molar concentration | mol/m³ | Quantity / Volume |
| `CatalyticConcentration` | catalytic activity concentration | kat/m³ | CatalyticActivity / Volume |
| `ReactionRate` | reaction rate | mol/(m³·s) | Concentration / Duration |

### Dimensionless & counting

| Class | Quantity | SI unit | Notes |
|-------|----------|---------|-------|
| `Ratio` | dimensionless ratio | 1 | percent, ppm, ppb, ppt, dB helpers |

> **Reciprocal helpers.** Reciprocal quantity pairs expose direct converters rather than an
> operator: `ElectricResistance.ToElectricConductance()` ⇄ `ElectricConductance.ToElectricResistance()`
> (G = 1/R), and `Resistivity.ToConductivity()` ⇄ `Conductivity.ToResistivity()` (σ = 1/ρ).

> **Operator caveat.** A few classes omit their defining arithmetic operator: `Torque` (N·m)
> would clash with `Energy` (joule, same signature `Force × Length`); `Wavenumber` uses
> `FromWavelength`/`ToWavelength` instead of `1 / Length`; and `ThermalConductivity` and
> `Radiance` compose from three independent factors, so they are constructed via their
> `From…` factories.

> Amount / count of entities is modeled by the foundational
> [`Quantity`](#foundational-units) type (canonical unit: raw count), not here.
