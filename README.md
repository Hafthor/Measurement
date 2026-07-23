# Measurement

A library of strongly-typed measurement classes. Each class stores its value
internally in a single canonical (SI) unit and exposes `FromXxx` factory
methods and `ToXxx` accessors for every supported unit.

## Conventions

- Each measurement is an immutable **`readonly partial struct`** (a value type — no heap
  allocation, and `default(Length)` is `0 m`) that stores a single canonical `double` in the
  **canonical SI unit** (e.g. `Length` stores metres). A **Roslyn source generator**
  (`Measurement.Generators`) emits the identical-across-every-type surface from a
  `[Measurement("m")]` attribute: value equality (`Equals`/`GetHashCode`/`==`/`!=`, plus
  `NearlyEquals` for ULP-tolerant checks), ordering (`IComparable<T>`, `< > <= >=`), formatting
  (`ToString`, `IFormattable`), the same-type `+`/`-`/negation operators, scalar math
  (`* k`, `/ k`, and same-type `/` → ratio), `Abs`/`Min`/`Max`/`Clamp`/`Lerp`, and the
  `IMeasurement<T>` / `System.Numerics` implementation. Each type's hand-written source only
  contains its `FromXxx`/`ToXxx` unit methods and cross-type operators.
- Construction is via `public static T FromUnit(double value)` factory methods.
- Read-out is via `public double ToUnit()` methods.
- `ToString()` renders the canonical value with the standard SI unit symbol, e.g.
  `Force.FromNewtons(6).ToString()` → `"6 N"`, `Speed.FromMetersPerSecond(10)` → `"10 m/s"`.
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
read-out; SI-prefixed decades come from the prefix chain. Two unit names shared by two
quantities — `JouleSeconds` (`Action`/`AngularMomentum`) and `RevolutionsPerMinute`
(`Frequency`/`AngularVelocity`) — are ambiguous as bare input, so use their explicit
`FromJouleSeconds`/`FromRevolutionsPerMinute` factories there (read-out is unaffected).
The explicit `FromKilometers`/`ToMilligrams`-style methods remain on every type regardless.
Requires C# 14 / .NET 10 (extension members).

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
| `Quantity` | amount of substance / count | mole | mol |
| `LuminousIntensity` | luminous intensity | candela | cd |

> **`Mass`** is stored canonically in **grams**, not the official SI base unit (the
> kilogram). Anchoring on the gram keeps whole-gram values exact in IEEE-754 and makes
> `FromGrams`/`ToGrams` an identity round-trip; kilogram values still round-trip exactly
> when whole (2 kg ⇄ 2000 g), and only fractional-kilogram factors pick up the usual
> floating-point slop. `From/To` methods for kilograms, tonnes, pounds, etc. are all still
> provided, and `ToString()` renders in grams (e.g. `Mass.FromKilograms(2)` → `"2000 g"`).

> **`Duration`** is preferred over `Time` to avoid clashing with `System.DateTime`/`TimeSpan`
> semantics, to make clear it represents an *elapsed* quantity, and because we want it to
> cover the full range from Planck time up to astronomical/cosmological scales
> (nanoseconds → seconds → days → Julian years → millennia → Hubble time, etc.). Note that `Duration`
> is **NOT** suited for exact calendaring operations (such as adding months or years).

> **`Quantity`** models amount of substance and plain counts. Although the mole is really a
> *count* of elementary entities, chemists carry it through equations like a unit (mol/L,
> g/mol, …), so it earns its own dimensioned type. The canonical unit is the mole; raw
> counts, dozens, gross, etc. relate to it via Avogadro's number (kept as an inline factor
> in the class). E.g. `Quantity.FromMoles(n)`, `Quantity.FromCount(n)`.

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
| `Density` | mass density | kg/m³ | Mass / Volume |
| `SpecificVolume` | specific volume | m³/kg | Volume / Mass |
| `LinearDensity` | linear mass density | kg/m | Mass / Length |
| `AreaDensity` | area (surface) density | kg/m² | Mass / Area |
| `MassFlowRate` | mass flow rate | kg/s | Mass / Duration |
| `Molality` | molality | mol/kg | Quantity / Mass |
| `MolarMass` | molar mass | kg/mol | Mass / Quantity |

### Mechanics & dynamics

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Momentum` | linear momentum, impulse | kg·m/s | Mass × Speed |
| `AngularMomentum` | angular momentum | kg·m²/s | MomentOfInertia × AngularVelocity |
| `Torque` | torque, moment of force | N·m | Force × Length |
| `MomentOfInertia` | moment of inertia | kg·m² | Mass × Area |
| `SurfaceTension` | surface tension | N/m | Force / Length |
| `DynamicViscosity` | dynamic viscosity | Pa·s | Pressure × Duration |
| `KinematicViscosity` | kinematic viscosity | m²/s | Area / Duration |
| `Action` | action | J·s | Energy × Duration |

### Thermodynamics

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `HeatCapacity` | heat capacity, entropy | J/K | Energy / Temperature |
| `SpecificHeatCapacity` | specific heat capacity | J/(kg·K) | HeatCapacity / Mass |
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
| `ElectricDipoleMoment` | electric dipole moment | C·m | ElectricCharge × Length |

### Photometry & radiation

| Class | Quantity | SI unit | Composition |
|-------|----------|---------|-------------|
| `Luminance` | luminance | cd/m² | LuminousIntensity / Area |
| `LuminousEnergy` | luminous energy | lm·s | LuminousFlux × Duration |
| `LuminousExposure` | luminous exposure | lx·s | Illuminance × Duration |
| `Radiance` | radiance | W/(m²·sr) | Power / (Area × SolidAngle) |
| `RadiantIntensity` | radiant intensity | W/sr | Power / SolidAngle |
| `Exposure` | radiation exposure | C/kg | ElectricCharge / Mass |
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
| `Ratio` | dimensionless ratio | 1 | percent, ppm, ppb, dB helpers |
| `PlaneAngle` | see `Angle` above | rad | |

> **Operator caveat.** A few classes omit their defining arithmetic operator: `Torque` (N·m)
> would clash with `Energy` (joule, same signature `Force × Length`); `Wavenumber` uses
> `FromWavelength`/`ToWavelength` instead of `1 / Length`; and `ThermalConductivity` and
> `Radiance` compose from three independent factors, so they are constructed via their
> `From…` factories.

> Amount / count of entities is modelled by the foundational
> [`Quantity`](#foundational-units) type (unit: mole), not here.
