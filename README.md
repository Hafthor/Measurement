# Measurement

A library of strongly-typed measurement classes. Each class stores its value
internally in a single canonical (SI) unit and exposes `FromXxx` factory
methods and `ToXxx` accessors for every supported unit.

## Conventions

- Each measurement is an immutable **`readonly partial struct`** (a value type — no heap
  allocation, and `default(Length)` is `0 m`) that stores a single canonical `double`. That
  canonical unit is usually the SI unit, but many types **anchor on a finer sub-unit** for
  IEEE-754 precision (see [Storage anchoring](#storage-anchoring--precision)); the private
  field is named for the actual stored unit. A **Roslyn source generator**
  (`Measurement.Generators`) emits the identical-across-every-type surface from a
  `[Measurement("m")]` attribute (with optional `VariableName` for the stored-field name and
  `DisplayFactor` for the store→display scale): value equality (`Equals`/`GetHashCode`/`==`/`!=`, plus
  `NearlyEquals` for ULP-tolerant checks), ordering (`IComparable<T>`, `< > <= >=`), formatting
  and parsing (`ToString`/`IFormattable`, `Parse`/`TryParse` via `IParsable<T>`/`ISpanParsable<T>`),
  the same-type `+`/`-`/negation operators, scalar math
  (`* k`, `/ k`, and same-type `/` → ratio), `Abs`/`Min`/`Max`/`Clamp`/`Lerp`, and the
  `IMeasurement<T>` / `System.Numerics` implementation. From a declarative list of
  **`[SiUnit]`/`[Unit]` attributes** it also generates each type's `FromXxx`/`ToXxx` unit methods
  and the fluent-prefix hooks (see [Fluent SI prefixes](#fluent-si-prefixes)): `[SiUnit("Grams", 6,
  "Kilo None Milli")]` expands a metric family (a base factor of `10^6` relative to the stored
  anchor, one method per listed prefix), while `[Unit("Pounds", 453.59237e6)]` declares a single
  non-metric unit by its factor (with an optional `Offset` for affine scales like temperature). So
  a type's hand-written source only **declares its units** via these attributes plus its cross-type
  operators — the `FromXxx`/`ToXxx` bodies are generated.
- Construction is via `public static T FromUnit(double value)` factory methods.
- Read-out is via `public double ToUnit()` methods.
- `ToString()` renders the value in its **fundamental SI unit symbol** (converting from the
  stored anchor via `DisplayFactor`), e.g. `Force.FromNewtons(6).ToString()` → `"6 N"`,
  `Speed.FromMetersPerSecond(10)` → `"10 m/s"`. The one exception is `Mass`, which shows
  grams (`"2000 g"`) rather than the SI kilogram.
- Composite measurements are defined in terms of the foundational ones and expose C#
  arithmetic operators relating them, so you can write e.g. `Speed speed = length / duration;`,
  `Force f = mass * acceleration;`, or `Energy e = force * length;` directly.
- Every measurement supports same-type `+`, `-`, and unary negation
  (e.g. `Length total = a + b;`). This includes **`Temperature`**: its canonical unit is
  kelvin — an absolute (true-zero) scale — so the arithmetic is well-defined. Just note that
  a sum read back on an offset scale looks shifted (`0 °C + 0 °C` = 546.30 K = 273.15 °C).
- Every measurement implements `IMeasurement<T>`, opting into **`System.Numerics` generic
  math** — `IAdditionOperators`, `ISubtractionOperators`, `IUnaryNegationOperators`,
  `IAdditiveIdentity` (`T.AdditiveIdentity` / `T.Zero`), `IComparisonOperators`, and scalar
  `IMultiply`/`IDivisionOperators` — so you can write generic algorithms like
  `T Sum<T>(IEnumerable<T> xs) where T : IMeasurement<T>`.

## Fluent SI prefixes

Alongside the explicit `FromXxx`/`ToXxx` API, there is a fluent interface for SI prefixes so
you never hand-write `FromKilo…`/`FromMilli…` per class. All 24 SI prefixes (`Quetta`…`Quecto`)
are defined once and stack.

**Construction** — the always-available, non-`double` entry point (no opt-in needed). Each
class exposes a direct hook for its **base SI unit** and its **non-SI units**; the SI-prefixed
decades (kilo, milli, …) are expressed through the prefix chain rather than as separate
`Kilometers`/`Milligrams` members, and prefixes stack:

```csharp
Mass    m = Measure.Of(5).Kilo.Grams;      // 5 kg  (mass prefixes attach to the gram)
Length  d = Measure.Of(3).Kilo.Meters;     // 3 km  (not .Kilometers — use the chain)
Length  y = Measure.Of(1).Miles;           // non-SI unit → direct hook
Energy  e = Measure.Of(2).Mega.Joules;     // 2 MJ
Length  x = Measure.Of(1).Mega.Mega.Meters; // prefixes stack → 1e12 m
```

**Read-out** — base/non-SI unit, optionally prefixed, returns a `double`:

```csharp
double lb = Measure.Of(5).Kilo.Grams.To.Pounds;   // 5 kg expressed in pounds
double km = someLength.To.Kilo.Meters;            // metres → km
double ms = duration.To.Milli.Seconds;            // seconds → ms
double f  = temperature.To.Fahrenheit;            // non-SI reader
```

**Opt-in `double` sugar** — for `5.0.Kilo.Meters` directly on numbers, add the import. It is
scoped to a separate namespace so it never pollutes `double` unless you ask for it:

```csharp
using com.hafthor.Measurement.Fluent;   // per file, or in a GlobalUsings.cs

Mass m = 5.0.Kilo.Grams;                 // only compiles where this using is present
```

Direct unit hooks cover each class's base SI unit and its non-SI units for both input and
read-out; SI-prefixed decades come from the prefix chain. The entire fluent surface (input hooks,
the `To` read-out builder, and output hooks) is **generated** from the same `[SiUnit]`/`[Unit]`
declarations, so it stays in sync with the `FromXxx`/`ToXxx` methods automatically. A few rules the
generator applies:

- **Squared/cubed metric units** whose factor can't be reproduced by a single chained prefix —
  `SquareKilometers`, `SquareCentimeters`, `SquareMillimeters`, `CubicCentimeters`,
  `CubicMillimeters` — keep their own bare hooks (e.g. 1 cm² = 1e-4 m², not the 1e-2 a lone `Centi`
  would give).
- **Prefixed aliases** — a declared unit whose name is just an SI prefix plus another unit
  (`Kilomoles` = `Kilo` + `Moles`, `Kilocalories`, `MilliampereHours`, `KilogramSquareMeters`, …) —
  get **no** fluent hook; reach them through the chain (`Measure.Of(1).Kilo.Moles`). Their explicit
  `FromKilomoles`/`ToKilomoles` factories still exist.
- **Shared names** — a unit name owned by two quantities (`JouleSeconds` on `Action`/`AngularMomentum`,
  `RevolutionsPerMinute` on `Frequency`/`AngularVelocity`) would be ambiguous as a bare input hook, so
  it resolves to a **selector** that names the measurement: `1.0.JouleSeconds.Action`,
  `Measure.Of(3).RevolutionsPerMinute.Frequency`. Read-out is unambiguous and stays direct
  (`freq.To.RevolutionsPerMinute`).

The explicit `FromKilometers`/`ToMilligrams`-style methods remain on every type regardless.
Requires C# 14 / .NET 10 (extension members).

## JSON serialization

`System.Text.Json` support ships in the box. Register `MeasurementJsonConverterFactory` once and
every measurement type serializes in its **fundamental SI unit** (stable and human-readable —
independent of the internal storage anchor); dimensionless types (`Ratio`, `Quantity`) serialize
as a bare number.

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
suffix is validated-if-present, optional otherwise). The converter is fully generic — it works for
any `[Measurement]` type, including new ones, via `IMeasurement<T>` and the type's `DisplayFactor`.

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
and is rejected. The symbol→type registry is built once by reflection, so new `[Measurement]` types
are picked up automatically.

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

> **`Mass`** is stored canonically in **micrograms** (not the official SI base unit, the
> kilogram). A `double` represents every integer exactly up to 2⁵³ ≈ 9.0×10¹⁵, so anchoring
> on the microgram keeps microgram/milligram/gram-scale values — and any finer decimal that
> is a whole number of micrograms, e.g. `0.1 mg` = `100 µg` — exact in IEEE-754, at the cost
> of losing *integer* exactness above ~9,000 t (relative precision, ~15–16 digits, is
> unchanged). See **Storage anchoring & precision** below.

### Storage anchoring & precision

Most quantities anchor their stored `double` on a unit **at or below** their SI unit, so common
small-scale decimals become exact integer counts of the anchor (e.g. `0.1 µF` = `100 pF`, exact).
Because a `double` is integer-exact only up to 2⁵³ ≈ 9.0×10¹⁵, each anchor also sets the ceiling
above which *integer* exactness is lost (relative precision, ~15–16 digits, is never affected).
The stored field is named for its anchor unit (the `VariableName`), and `ToString()` always
renders the value in its **fundamental SI unit** by dividing out the `DisplayFactor` — so storage
is an internal precision detail. The sole display exception is `Mass`, which prints grams rather
than the SI kilogram.

Headline anchors (with the integer-exact ceiling that matters in practice):

| Type | stored unit | `ToString()` unit | exact-integer ceiling |
|------|-------------|-------------------|-----------------------|
| `Length` | nanometre (nm) | metre (m) | ~9.0×10⁶ m (AU/ly become approximate) |
| `Mass` | microgram (µg) | gram (g) | ~9,000 t |
| `Volume` | microlitre (µL) | cubic metre (m³) | ~9,000 m³ |
| `ElectricCurrent` | microampere (µA) | ampere (A) | ~9.0×10⁹ A |
| `Voltage` | microvolt (µV) | volt (V) | ~9.0×10⁹ V |
| `Quantity` | count (entities) | — (bare number) | 2⁵³ ≈ 9.0×10¹⁵ entities |
| `Ratio` | parts-per-trillion (ppt) | — (bare ratio) | ~9.0×10³ (ratio) |
| `Capacitance` | picofarad (pF) | farad (F) | ~9,000 F |
| `Inductance` | nanohenry (nH) | henry (H) | ~9.0×10⁶ H |
| `ElectricCharge` | nanocoulomb (nC) | coulomb (C) | ~9.0×10⁶ C |
| `ElectricConductance` | nanosiemens (nS) | siemens (S) | ~9.0×10⁶ S |
| `MagneticFlux` | nanoweber (nWb) | weber (Wb) | ~9.0×10⁶ Wb |
| `MagneticFluxDensity` | nanotesla (nT) | tesla (T) | ~9.0×10⁶ T |

Many derived quantities are anchored the same way (stored anchor → SI display), so their sub-SI
inputs stay exact too:

| Type | stored anchor | `ToString()` unit |
|------|---------------|-------------------|
| `Area` | square millimetre (mm²) | m² |
| `Angle` | arcsecond (″) | rad |
| `AngularVelocity` | degree/second | rad/s |
| `AngularAcceleration` | degree/second² | rad/s² |
| `Action` | nanojoule-second (nJ·s) | J·s |
| `AbsorbedDose` | microgray (µGy) | Gy |
| `EquivalentDose` | microsievert (µSv) | Sv |
| `DoseRate` | milligray/hour | Gy/s |
| `CatalyticActivity` | nanokatal (nkat) | kat |
| `Concentration` | micromole/litre | mol/m³ |
| `Conductivity` | millisiemens/centimetre | S/m |
| `Resistivity` | microohm-centimetre (µΩ·cm) | Ω·m |
| `DynamicViscosity` | millipascal-second (mPa·s) | Pa·s |
| `KinematicViscosity` | centistokes (cSt) | m²/s |
| `Illuminance` | millilux (mlx) | lx |
| `LuminousFlux` | millilumen (mlm) | lm |
| `LuminousIntensity` | millicandela (mcd) | cd |
| `HeatFluxDensity` | milliwatt/m² | W/m² |
| `ThermalConductivity` | milliwatt/(m·K) | W/(m·K) |
| `SurfaceTension` | millinewton/metre | N/m |
| `Torque` | newton-millimetre (N·mm) | N·m |
| `SpecificVolume` | cubic millimetre/gram | m³/g |
| `VolumetricFlowRate` | cubic millimetre/second | m³/s |
| `LinearMagneticFluxDensity` | nanoweber/metre (nWb/m) | Wb/m |

All other types store (and display) their SI unit. `Duration` and `Temperature` are deliberately
left on seconds / kelvin. Every `From*`/`To*` method is provided regardless of the anchor, and
cross-type operators read through the `To*` accessors so results are unaffected.


> **`Duration`** is preferred over `Time` to avoid clashing with `System.DateTime`/`TimeSpan`
> semantics, to make clear it represents an *elapsed* quantity, and because we want it to
> cover the full range from Planck time up to astronomical/cosmological scales
> (nanoseconds → seconds → days → Julian years → millennia → Hubble time, etc.). Note that `Duration`
> is **NOT** suited for exact calendaring operations (such as adding months or years).

> **`Quantity`** models amount of substance and plain counts. Although the mole is really a
> *count* of elementary entities, chemists carry it through equations like a unit (mol/L,
> g/mol, …), so it earns its own dimensioned type. The canonical unit is the **raw count**, so
> integer counts (and whole pairs, dozens, gross) are exact; moles are carried as
> count ÷ Avogadro's number and are the approximate side (the two can't both be exact). Because
> it is fundamentally a dimensionless count, `ToString()` prints the **bare number** (no
> symbol). E.g. `Quantity.FromMoles(n)`, `Quantity.FromCount(n)`.

> **Temperature** is a single `Temperature` class (kelvin canonical); Celsius, Fahrenheit,
> Rankine, etc. are exposed as `From/To` methods on it rather than as separate classes.

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
