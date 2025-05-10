# Arbeitszeitrechner

[![Build status](https://img.shields.io/github/actions/workflow/status/Abrynos/Arbeitszeitrechner/ci.yml?branch=main&label=Build&logo=github)](https://github.com/Abrynos/Arbeitszeitrechner/actions)
[![GitHub release version](https://img.shields.io/github/v/release/Abrynos/Arbeitszeitrechner?label=Stable&logo=github)](https://github.com/Abrynos/Arbeitszeitrechner/releases/latest)
[![License](https://img.shields.io/github/license/Abrynos/Arbeitszeitrechner)](https://github.com/Abrynos/Arbeitszeitrechner/blob/master/LICENSE)

# Description

This is a simple application serving the purpose of helping you calculate your work-hours in a flexi-time model. Based on your starting-time and the configured "normal work hours", it computes how long you have to work today to meet your quota and how long you have already worked at the current point in time.

Calculations are based on several assumptions:
- The Austrian legal system, where a worker is required to take at least a 30-minute break after 6 hours of work.
- Normal work hours of 08:15 hours from Monday to Thursday and 5:30 hours on a Friday.
  - Standard hours are subject to configuration in the file `app.db`.

# Translation

I am open to translating the app to any known language. As of now the easiest way is to create a [merge request](https://github.com/Abrynos/Arbeitszeitrechner/pulls) adding the strings in source-code directly.

# Collaboration

If you would like to contribute but do not have a specific idea yet, feel free to look at the list of [previously suggested ideas](https://github.com/Abrynos/Arbeitszeitrechner/issues?q=label%3A%22%F0%9F%91%8D+merge+request+okay%22). If you already have a specific idea but you are unable to find it in the list of previous suggestions you can either open an issue with your feature request or open a merge request directly.
