# Changelog

All notable changes to this project will be documented in this file. See [standard-version](https://github.com/conventional-changelog/standard-version) for commit guidelines.

### [3.0.3](https://github.com/joelbrinkley/NetStreams/compare/v3.0.2...v3.0.3) (2021-08-24)

### [3.0.2](https://github.com/joelbrinkley/NetStreams/compare/v3.0.1...v3.0.2) (2021-08-23)

### [3.0.1](https://github.com/joelbrinkley/NetStreams/compare/v3.0.0...v3.0.1) (2021-08-20)


### Bug Fixes

* look up the the event using the correct name ([8c705de](https://github.com/joelbrinkley/NetStreams/commit/8c705ded8ec4a367301a327539ef428c2b2761d9))

## 3.0.0 (2021-08-17)


### ⚠ BREAKING CHANGES

* Middleware to enable message correlation through the kafka header (#52)

### Features

* Middleware to enable message correlation through the kafka header ([#52](https://github.com/joelbrinkley/NetStreams/issues/52)) ([fa258e6](https://github.com/joelbrinkley/NetStreams/commit/fa258e601a00069c14a9aee48cdd8974b2749294))

## 2.1.0 (2021-07-28)


### Features

* add authentication methods ([#51](https://github.com/joelbrinkley/NetStreams/issues/51)) ([da36835](https://github.com/joelbrinkley/NetStreams/commit/da36835c1e02661b3796d34dd0bb76cf118b0bed))

### 2.0.1 (2021-07-27)

## 2.0.0-rc.15 (2021-07-22)

## 2.0.0-rc.14 (2021-07-22)

## 2.0.0-rc.13 (2021-07-21)


### Bug Fixes

* Consume Commit Behavior not calling commit ([4666e0b](https://github.com/joelbrinkley/NetStreams/commit/4666e0b72bb98d6aa5a2634c1e2abc64c9d35db8))

## 2.0.0-rc.12 (2021-07-14)


### Features

* added toggle for adding header ([#43](https://github.com/joelbrinkley/NetStreams/issues/43)) ([6605070](https://github.com/joelbrinkley/NetStreams/commit/6605070d2047b70188d8c2d0c3e3ddf8fd34e5be))

## 2.0.0-rc.11 (2021-06-25)

## 2.0.0-rc.10 (2021-06-25)


### Features

* add offset to request operation name and move client metric lag before processing next ([#41](https://github.com/joelbrinkley/NetStreams/issues/41)) ([14f7956](https://github.com/joelbrinkley/NetStreams/commit/14f7956ab7ee9a421359517aafea435a83ab1e5d))

## 2.0.0-rc.9 (2021-06-14)


### Features

* Configure auto offset reset values, default to latest ([#40](https://github.com/joelbrinkley/NetStreams/issues/40)) ([3c94f8a](https://github.com/joelbrinkley/NetStreams/commit/3c94f8a5cb9b1affbe3a0c16060ddac145576722))

## 2.0.0-rc.8 (2021-05-13)

## 2.0.0-rc.7 (2021-05-13)


### Features

* skip malformed messages ([#38](https://github.com/joelbrinkley/NetStreams/issues/38)) ([e722506](https://github.com/joelbrinkley/NetStreams/commit/e722506be9ebb69a9a97c9d6e8aa59eb2db162e0))

## 2.0.0-rc.6 (2021-05-13)

## 2.0.0-rc.5 (2021-05-11)

## 2.0.0-rc.4 (2021-05-11)


### Features

* Add the ability to add custom logging ([#34](https://github.com/joelbrinkley/NetStreams/issues/34)) ([2e33eec](https://github.com/joelbrinkley/NetStreams/commit/2e33eec34a62c9d103b4c4ce6c99695bc63e75ef))

## 2.0.0-rc.3 (2021-05-11)

## 2.0.0-rc.2 (2021-05-11)


### Bug Fixes

* push item onto stack ([5bbb9a6](https://github.com/joelbrinkley/NetStreams/commit/5bbb9a66d40c21b67f13614229d3841bb24045d0))

## 2.0.0-rc.1 (2021-05-11)


### Bug Fixes

* some steps not await next and returning result ([463d539](https://github.com/joelbrinkley/NetStreams/commit/463d5392e9690851311b0f67fbbef44d594b5437))

## 2.0.0-rc.0 (2021-05-10)


### ⚠ BREAKING CHANGES

* Add application insights pipeline steps (#29)

### Features

* Add application insights pipeline steps ([#29](https://github.com/joelbrinkley/NetStreams/issues/29)) ([d0ff565](https://github.com/joelbrinkley/NetStreams/commit/d0ff5659c9a4338c6ce35a449d085ec98bf250ec))

## 1.3.0-rc.4 (2021-05-06)

## 1.3.0-rc.3 (2021-04-26)


### Bug Fixes

* Configure Key instead of Keystore. Configure Security on TopicCreator ([#24](https://github.com/joelbrinkley/NetStreams/issues/24)) ([bd00e66](https://github.com/joelbrinkley/NetStreams/commit/bd00e66a48103156d13d70ccae732284d171ac8b))

## 1.3.0-rc.2 (2021-04-23)


### Bug Fixes

* Set SslKeystorePassword, not SslKeyPassword ([#23](https://github.com/joelbrinkley/NetStreams/issues/23)) ([5450d84](https://github.com/joelbrinkley/NetStreams/commit/5450d84740f89a27b65998d3233c14c0bb66316f))

## 1.3.0-rc.1 (2021-04-23)


### Features

* ssl cert passwords ([#22](https://github.com/joelbrinkley/NetStreams/issues/22)) ([0e73d82](https://github.com/joelbrinkley/NetStreams/commit/0e73d82538a0bf5615f19e1150dfc6431bfe7a0f))

## 1.3.0-rc.0 (2021-04-23)


### Features

* handle and handle async support ([#21](https://github.com/joelbrinkley/NetStreams/issues/21)) ([6375dc5](https://github.com/joelbrinkley/NetStreams/commit/6375dc55ceecdaf79b0cfb71693229b3133e6cfc))

### 1.2.1-rc.1 (2021-04-22)

### 1.2.1-rc.0 (2021-04-22)

## 1.2.0 (2021-04-21)


### Features

* configure default delivery mode to be at least ([#18](https://github.com/joelbrinkley/NetStreams/issues/18)) ([1f853ac](https://github.com/joelbrinkley/NetStreams/commit/1f853acd44b89e31c6f532eef29443ff7d0a474d))

### 1.1.2 (2021-04-20)

### 1.1.1 (2021-04-19)


### Bug Fixes

* refactor test to not be flakey ([a5d2997](https://github.com/joelbrinkley/NetStreams/commit/a5d29977f42242bd9879c315efca645cb1b1c52c))

## 1.1.0 (2021-04-16)


### Features

* add nullable resolve key ([#8](https://github.com/joelbrinkley/NetStreams/issues/8)) ([1f8841c](https://github.com/joelbrinkley/NetStreams/commit/1f8841c0061528f354887766812b3586698ec82a))

### 1.0.1 (2021-04-16)
