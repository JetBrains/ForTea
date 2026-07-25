package com.jetbrains.fortea.lexer

import com.jetbrains.rider.test.annotations.Solution
import com.jetbrains.rider.test.shared.constants.TeamCityTags
import org.junit.jupiter.api.Tag
import org.junit.jupiter.api.Test

@Tag(TeamCityTags.Plugins.ForTea.General)
@Solution("ProjectWithT4")
class T4ExecutableOutputDependentLexerTest : T4OutputDependentLexerTestBase() {
  @Test fun `test basic C# highlighting`() { doTest() }
  @Test fun `test basic HTML highlighting`() { doTest() }
  @Test fun `test that extension can contain dot`() { doTest() }
  @Test fun `test default extension in executable template`() { doTest() }
}