# Contributing

## Implementation boundary

Development implementation is authorized as of 4 November 2026. Changes that alter requirements, workflows, database constraints, API contracts, or UI behavior must first be documented in the relevant SDD and approved before being treated as a baseline.

## Documentation change process

1. Identify the affected SDD.
2. Update the document version and change history.
3. Mark assumptions and unresolved decisions explicitly.
4. Submit a pull request.
5. Obtain review and approval before treating a change as baseline.

## Sensitive data

Never commit actual patient data, staff credentials, secrets, private keys, production database dumps, or real clinical reports. Use synthetic test data only.

## Production boundary

Development code and disposable non-production validation are permitted. Production database execution, real patient-data loading, and deployment require separate explicit authorization.
