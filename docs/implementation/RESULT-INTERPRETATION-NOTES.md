# Result Interpretation — Implementation Notes

## Confirmed behavior

The application supports two result value families:

- **Numeric:** decimal result compared with the selected effective reference interval. Values below the lower bound are LOW; values above the upper bound are HIGH; values within the applicable inclusive bounds are NORMAL. If no usable interval is selected, the value is UNCLASSIFIED.
- **Qualitative:** selected option is interpreted from the laboratory-configured catalog option. The application must not hardcode that terms such as Reactive, Positive, or Detected always mean the same clinical interpretation across all tests.

## Source-schema alignment

The current SQL baseline includes `TestParameterDefinitions.ValueType` values Numeric, Qualitative, Text, and Titer; `ReferenceIntervals` with sex/age applicability and numeric bounds; `ResultValueOptions` with `IsAbnormal`; and immutable `TestResultRevisions` with value snapshots, abnormal/critical flags, status, and encoder metadata.

The current pure domain helper is an early scaffold. It is not a replacement for full SDD-03 mapping, reference-interval selection, test-method handling, age/sex applicability, effective-date selection, revision persistence, or review/verification workflow.

## Clinical governance boundary

No reference ranges, critical thresholds, test option catalogs, or interpretation policies are supplied by the software. The laboratory must approve those values and policies. A numerical result can only be classified after the correct published interval is selected for the patient, method, unit, and applicable date. A qualitative result can only be classified using the published option configured for that test parameter.

## Contract boundary

SDD-11 does not define the result-entry, revision, submission, verification, critical-result, or report APIs. Those contracts and UI acceptance criteria must be specified and approved before those workflows are implemented. SDD-12 explicitly deferred their detailed test suites; add test cases in a controlled addendum before acceptance.
