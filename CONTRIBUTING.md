# Contributing

Thank you for taking the time to read this contributing guide and for showing interest in helping this project!

## Getting Started

Need a helping hand to get started? Check out these resources!

- [Discord Server][discord]
- [Cubusky Website][website]

We follow the [Contributor Covenant][covenant] as our code of conduct.

In short:

> We as members, contributors, and leaders pledge to make participation in our community a harassment-free experience for everyone, regardless of age, body size, visible or invisible disability, ethnicity, sex characteristics, gender identity and expression, level of experience, education, socio-economic status, nationality, personal appearance, race, caste, color, religion, or sexual identity and orientation.

## Coding Guidelines

This project uses [Husky.Net][husky] to run git hooks. Once you have cloned the repository, build the project and husky should be installed automatically. If you have any issues, please refer to the [Husky.Net documentation][husky] for help.

All of our code is formatted with [CSharpier][csharpier] on pre-commit. If your code looks different after committing, this is why. For a more comfortable coding experience, we recommend installing the [CSharpier plugin][csharpier-editors] for your IDE and enabling format on save, so your code is automatically formatted on save as well.

All our code is written in US English and spellchecked with [CSpell][cspell] in our CI/CD pipeline. If there are any spelling errors, these should be caught during pre-commit. If you use any non-English words, please add them to the `cspell.json` file in the root of the repository.

If you need support or guidance, feel free to reach out to us on our [Discord Server][discord]. We're glad to help!

<!-- Links -->

[discord]: https://discord.gg/c979gvkHdb
[website]: https://cubusky.code
[covenant]: https://www.contributor-covenant.org/version/2/1/code_of_conduct/
[husky]: https://alirezanet.github.io/Husky.Net/
[csharpier]: https://csharpier.com/
[csharpier-editors]: https://csharpier.com/docs/Editors
[cspell]: https://cspell.org/