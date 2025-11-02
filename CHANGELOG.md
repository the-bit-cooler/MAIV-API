# Changelog

## [1.1.1](https://github.com/the-bit-cooler/MAIV-API/compare/maiv-api-v1.1.0...maiv-api-v1.1.1) (2025-11-02)


### 🐛 Bug Fixes

* future-proof by returning user email from validated session token ([7e66405](https://github.com/the-bit-cooler/MAIV-API/commit/7e6640548527601c359972c09e7a2a85f063e4da))

## [1.1.0](https://github.com/the-bit-cooler/MAIV-API/compare/maiv-api-v1.0.1...maiv-api-v1.1.0) (2025-11-02)


### ✨ Features

* spoof up magic-link-login email content ([d525d85](https://github.com/the-bit-cooler/MAIV-API/commit/d525d855155f67b3e3fe70ebf8b69776c1067c5a))


### 🐛 Bug Fixes

* receive jwt as a bearer token instead of in request body ([ae4368e](https://github.com/the-bit-cooler/MAIV-API/commit/ae4368ecff0dca2b9a0df882026982ad2dec1daf))
* return session token as json ([1e1f3f8](https://github.com/the-bit-cooler/MAIV-API/commit/1e1f3f876f6b75cf0802ad7e0549bf63dd2ecd0a))
* the ValidateEmail should check for null or empty email string ([7724be5](https://github.com/the-bit-cooler/MAIV-API/commit/7724be5cc4b281eab88cd701adc708370f3f6fe4))


### ♻️ Refactors

* change login to post method only with email as json body ([63f886c](https://github.com/the-bit-cooler/MAIV-API/commit/63f886c67dd6deade6c181a3fa79cbc2d2fa1f30))
* return simple bad requests for all non-ok and let front end handle the messaging ([9437b54](https://github.com/the-bit-cooler/MAIV-API/commit/9437b54408a18f6a64d89ab28cf86e5d15ed7890))


### 🧰 Maintenance

* remove unused using statement ([5a85555](https://github.com/the-bit-cooler/MAIV-API/commit/5a85555384c045211a2aad72df3709fd58410b09))

## [1.0.1](https://github.com/the-bit-cooler/MAIV-API/compare/maiv-api-v1.0.0...maiv-api-v1.0.1) (2025-11-02)


### 🧰 Maintenance

* clean up old changelog ([7cf95f4](https://github.com/the-bit-cooler/MAIV-API/commit/7cf95f4524675593fdb7c72c39dce684bbce2b9d))

## [1.0.0](https://github.com/the-bit-cooler/MAIV-API/compare/maiv-api-v0.1.0...maiv-api-v1.0.0) (2025-11-02)


### ⚠ BREAKING CHANGES

* removed endpoint for summarizing bible chapter

### ✨ Features

* add endpoint for illustrating a bible verse ([bed4a03](https://github.com/the-bit-cooler/MAIV-API/commit/bed4a0385350e2a4e1dc9e22d158e34e3bbc2e62))
* add SendGrid package reference for email functionality ([a5cf06b](https://github.com/the-bit-cooler/MAIV-API/commit/a5cf06bdaca0c6db3adbafbcb7279b18de709245))
* implement email login functionality with magic link and session validation ([d7964e0](https://github.com/the-bit-cooler/MAIV-API/commit/d7964e0927b808f694d512c54ac5c5ed0cbc04e5))


### 🐛 Bug Fixes

* remove deprecated service handler ([a47c54f](https://github.com/the-bit-cooler/MAIV-API/commit/a47c54f40e1d2d658cb8dae8513f31309eb16800))
* remove endpoint for generating chapter image ([914f8b0](https://github.com/the-bit-cooler/MAIV-API/commit/914f8b0ef044b8d176bf467c15a810528a1d083c))
* remove release-type parameter from release-please action to force manifest mode ([21999ac](https://github.com/the-bit-cooler/MAIV-API/commit/21999ac89760e98747d8b6df9eaa537c926a50ab))
* removed endpoint for summarizing bible chapter ([44b10f6](https://github.com/the-bit-cooler/MAIV-API/commit/44b10f6c8bd2980559073d2dc1070f5fd6fda80f))
* reset version to 0.0.0 and change release type to node ([cffb08e](https://github.com/the-bit-cooler/MAIV-API/commit/cffb08e6ab4101a0c5d628e92676705a9de3e6af))
* reset version to 0.0.0 and update release-please config structure ([4f1eee8](https://github.com/the-bit-cooler/MAIV-API/commit/4f1eee83c292f8c87977ebe1b21fbac8ff8e8e9d))
* switch from database storage for azure storage for generated verse explanations ([e043208](https://github.com/the-bit-cooler/MAIV-API/commit/e043208e0470f972c515f454ce2059e7ab19a18b))
* switch from database storage to azure storage for new translation generation ([2d4cf18](https://github.com/the-bit-cooler/MAIV-API/commit/2d4cf1805a3444777c0ddac4c3a2c206dbe402d4))


### 📖 Documentation

* add Conventional Commits guide to improve commit message consistency ([9ec9b8b](https://github.com/the-bit-cooler/MAIV-API/commit/9ec9b8be1c70063e8ea5b69439fbf0979923d905))


### 🧰 Maintenance

* add config and manifest file parameters to release-please action ([5ff4a04](https://github.com/the-bit-cooler/MAIV-API/commit/5ff4a04136cdc6045c95bac3fb51a1f70d89343a))
* add configuration for release-please with changelog sections ([7dffca1](https://github.com/the-bit-cooler/MAIV-API/commit/7dffca1359b15f1c33085148965ac6aa4f2466e2))
* add getter for blob client ([4db0ca8](https://github.com/the-bit-cooler/MAIV-API/commit/4db0ca8fc7c829bdad78250a6eb40e02bbb46505))
* add script to generate JWT secret ([bde6725](https://github.com/the-bit-cooler/MAIV-API/commit/bde672586c1b92226326832484d3ad5aaf509f6d))
* add service handler docs ([be0c70b](https://github.com/the-bit-cooler/MAIV-API/commit/be0c70bce3992520bcef89953c4f13f3e07dfeb1))
* **deps:** update Microsoft.Azure.Cosmos and Microsoft.Azure.Functions.Worker ([f875259](https://github.com/the-bit-cooler/MAIV-API/commit/f8752598403fa0edd3c5dbfee00a6053a4423cb6))
* fix caller id ([51b3816](https://github.com/the-bit-cooler/MAIV-API/commit/51b3816760577329c01fd3b85d29e0014f40e611))
* init project ([2437dac](https://github.com/the-bit-cooler/MAIV-API/commit/2437dacc9cce855dac8c5dee2418865a62f20632))
* **main:** release 1.0.0 ([#1](https://github.com/the-bit-cooler/MAIV-API/issues/1)) ([f4cb26e](https://github.com/the-bit-cooler/MAIV-API/commit/f4cb26ef1439d28383861073e8afdeecad7fbbd0))
* **main:** release 2.0.0 ([#2](https://github.com/the-bit-cooler/MAIV-API/issues/2)) ([0af6f9e](https://github.com/the-bit-cooler/MAIV-API/commit/0af6f9e425c85767bbdde9abb9b92f507ebf4491))
* **main:** release 2.1.0 ([#3](https://github.com/the-bit-cooler/MAIV-API/issues/3)) ([74bdde3](https://github.com/the-bit-cooler/MAIV-API/commit/74bdde35941bddf3d2e84bf4929c9f5cf710c232))
* release 1.0.0 ([59cab56](https://github.com/the-bit-cooler/MAIV-API/commit/59cab56a8141f19c7802e8b27fbb8a1f1c55f278))
* remove comment ([a0f6c20](https://github.com/the-bit-cooler/MAIV-API/commit/a0f6c20a8da1708c1dfec91b0904467885c0bebb))
* remove comment ([dc129ee](https://github.com/the-bit-cooler/MAIV-API/commit/dc129eed1838c17d444f785f78f3f6b4f75422b5))
* remove comment and unused mode param ([8ca943f](https://github.com/the-bit-cooler/MAIV-API/commit/8ca943f4f8e4ff140a3cafcca5118cc4e2644e72))
* remove unused using directives from AI and Data services ([b635152](https://github.com/the-bit-cooler/MAIV-API/commit/b6351526cd77a807afd2274f4bde7f977a51c949))
* restructure release-please config to move changelog-sections outside packages ([aeb6847](https://github.com/the-bit-cooler/MAIV-API/commit/aeb684715141b10a32bbffb408b8a697c4cb249e))
* standardize changelog-sections formatting in release-please config ([0a9f321](https://github.com/the-bit-cooler/MAIV-API/commit/0a9f321ee211cc3fe3f676c4c965dcb5ddcd66c5))
* use noun for storage folder name instead of verb ([4638f2d](https://github.com/the-bit-cooler/MAIV-API/commit/4638f2d33439aac09a600d51ec0f53b1c538ec7b))
