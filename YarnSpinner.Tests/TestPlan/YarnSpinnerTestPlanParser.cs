//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/desplesda/Work/YarnSpinner/YarnSpinner.Tests/TestPlan/YarnSpinnerTestPlan.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Yarn.Compiler {
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class YarnSpinnerTestPlanParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, COMMENT=14, WS=15, BOOL=16, IDENTIFIER=17, 
		VARIABLE=18, NUMBER=19, TEXT=20;
	public const int
		RULE_testplan = 0, RULE_run = 1, RULE_step = 2, RULE_hashtag = 3, RULE_lineExpected = 4, 
		RULE_optionExpected = 5, RULE_commandExpected = 6, RULE_stopExpected = 7, 
		RULE_actionSelect = 8, RULE_actionSet = 9, RULE_actionSetSaliencyMode = 10, 
		RULE_actionJumpToNode = 11;
	public static readonly string[] ruleNames = {
		"testplan", "run", "step", "hashtag", "lineExpected", "optionExpected", 
		"commandExpected", "stopExpected", "actionSelect", "actionSet", "actionSetSaliencyMode", 
		"actionJumpToNode"
	};

	private static readonly string[] _LiteralNames = {
		null, "'---'", "'#'", "'line:'", "'*'", "'option:'", "'[disabled]'", "'command:'", 
		"'stop'", "'select:'", "'set:'", "'='", "'saliency:'", "'node:'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, "COMMENT", "WS", "BOOL", "IDENTIFIER", "VARIABLE", "NUMBER", 
		"TEXT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "YarnSpinnerTestPlan.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static YarnSpinnerTestPlanParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public YarnSpinnerTestPlanParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public YarnSpinnerTestPlanParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class TestplanContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public RunContext[] run() {
			return GetRuleContexts<RunContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public RunContext run(int i) {
			return GetRuleContext<RunContext>(i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(YarnSpinnerTestPlanParser.Eof, 0); }
		public TestplanContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_testplan; } }
	}

	[RuleVersion(0)]
	public TestplanContext testplan() {
		TestplanContext _localctx = new TestplanContext(Context, State);
		EnterRule(_localctx, 0, RULE_testplan);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 24;
			run();
			State = 29;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==T__0) {
				{
				{
				State = 25;
				Match(T__0);
				State = 26;
				run();
				}
				}
				State = 31;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 32;
			Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class RunContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public StepContext[] step() {
			return GetRuleContexts<StepContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public StepContext step(int i) {
			return GetRuleContext<StepContext>(i);
		}
		public RunContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_run; } }
	}

	[RuleVersion(0)]
	public RunContext run() {
		RunContext _localctx = new RunContext(Context, State);
		EnterRule(_localctx, 2, RULE_run);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 35;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			do {
				{
				{
				State = 34;
				step();
				}
				}
				State = 37;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			} while ( (((_la) & ~0x3f) == 0 && ((1L << _la) & 14248L) != 0) );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class StepContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public LineExpectedContext lineExpected() {
			return GetRuleContext<LineExpectedContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public OptionExpectedContext optionExpected() {
			return GetRuleContext<OptionExpectedContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public CommandExpectedContext commandExpected() {
			return GetRuleContext<CommandExpectedContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public StopExpectedContext stopExpected() {
			return GetRuleContext<StopExpectedContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ActionSelectContext actionSelect() {
			return GetRuleContext<ActionSelectContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ActionSetContext actionSet() {
			return GetRuleContext<ActionSetContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ActionJumpToNodeContext actionJumpToNode() {
			return GetRuleContext<ActionJumpToNodeContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ActionSetSaliencyModeContext actionSetSaliencyMode() {
			return GetRuleContext<ActionSetSaliencyModeContext>(0);
		}
		public StepContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_step; } }
	}

	[RuleVersion(0)]
	public StepContext step() {
		StepContext _localctx = new StepContext(Context, State);
		EnterRule(_localctx, 4, RULE_step);
		try {
			State = 47;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case T__2:
				EnterOuterAlt(_localctx, 1);
				{
				State = 39;
				lineExpected();
				}
				break;
			case T__4:
				EnterOuterAlt(_localctx, 2);
				{
				State = 40;
				optionExpected();
				}
				break;
			case T__6:
				EnterOuterAlt(_localctx, 3);
				{
				State = 41;
				commandExpected();
				}
				break;
			case T__7:
				EnterOuterAlt(_localctx, 4);
				{
				State = 42;
				stopExpected();
				}
				break;
			case T__8:
				EnterOuterAlt(_localctx, 5);
				{
				State = 43;
				actionSelect();
				}
				break;
			case T__9:
				EnterOuterAlt(_localctx, 6);
				{
				State = 44;
				actionSet();
				}
				break;
			case T__12:
				EnterOuterAlt(_localctx, 7);
				{
				State = 45;
				actionJumpToNode();
				}
				break;
			case T__11:
				EnterOuterAlt(_localctx, 8);
				{
				State = 46;
				actionSetSaliencyMode();
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class HashtagContext : ParserRuleContext {
		public HashtagContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_hashtag; } }
	}

	[RuleVersion(0)]
	public HashtagContext hashtag() {
		HashtagContext _localctx = new HashtagContext(Context, State);
		EnterRule(_localctx, 6, RULE_hashtag);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 49;
			Match(T__1);
			State = 51;
			ErrorHandler.Sync(this);
			_alt = 1+1;
			do {
				switch (_alt) {
				case 1+1:
					{
					{
					State = 50;
					MatchWildcard();
					}
					}
					break;
				default:
					throw new NoViableAltException(this);
				}
				State = 53;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			} while ( _alt!=1 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER );
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class LineExpectedContext : ParserRuleContext {
		public LineExpectedContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_lineExpected; } }
	 
		public LineExpectedContext() { }
		public virtual void CopyFrom(LineExpectedContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class LineWithAnyTextExpectedContext : LineExpectedContext {
		[System.Diagnostics.DebuggerNonUserCode] public HashtagContext[] hashtag() {
			return GetRuleContexts<HashtagContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public HashtagContext hashtag(int i) {
			return GetRuleContext<HashtagContext>(i);
		}
		public LineWithAnyTextExpectedContext(LineExpectedContext context) { CopyFrom(context); }
	}
	public partial class LineWithSpecificTextExpectedContext : LineExpectedContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TEXT() { return GetToken(YarnSpinnerTestPlanParser.TEXT, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public HashtagContext[] hashtag() {
			return GetRuleContexts<HashtagContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public HashtagContext hashtag(int i) {
			return GetRuleContext<HashtagContext>(i);
		}
		public LineWithSpecificTextExpectedContext(LineExpectedContext context) { CopyFrom(context); }
	}

	[RuleVersion(0)]
	public LineExpectedContext lineExpected() {
		LineExpectedContext _localctx = new LineExpectedContext(Context, State);
		EnterRule(_localctx, 8, RULE_lineExpected);
		int _la;
		try {
			State = 71;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,6,Context) ) {
			case 1:
				_localctx = new LineWithSpecificTextExpectedContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 55;
				Match(T__2);
				State = 56;
				Match(TEXT);
				State = 60;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__1) {
					{
					{
					State = 57;
					hashtag();
					}
					}
					State = 62;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
				break;
			case 2:
				_localctx = new LineWithAnyTextExpectedContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 63;
				Match(T__2);
				State = 64;
				Match(T__3);
				State = 68;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
				while (_la==T__1) {
					{
					{
					State = 65;
					hashtag();
					}
					}
					State = 70;
					ErrorHandler.Sync(this);
					_la = TokenStream.LA(1);
				}
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class OptionExpectedContext : ParserRuleContext {
		public IToken isDisabled;
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TEXT() { return GetToken(YarnSpinnerTestPlanParser.TEXT, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public HashtagContext[] hashtag() {
			return GetRuleContexts<HashtagContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public HashtagContext hashtag(int i) {
			return GetRuleContext<HashtagContext>(i);
		}
		public OptionExpectedContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_optionExpected; } }
	}

	[RuleVersion(0)]
	public OptionExpectedContext optionExpected() {
		OptionExpectedContext _localctx = new OptionExpectedContext(Context, State);
		EnterRule(_localctx, 10, RULE_optionExpected);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 73;
			Match(T__4);
			State = 74;
			Match(TEXT);
			State = 78;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while (_la==T__1) {
				{
				{
				State = 75;
				hashtag();
				}
				}
				State = 80;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 82;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (_la==T__5) {
				{
				State = 81;
				_localctx.isDisabled = Match(T__5);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class CommandExpectedContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TEXT() { return GetToken(YarnSpinnerTestPlanParser.TEXT, 0); }
		public CommandExpectedContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_commandExpected; } }
	}

	[RuleVersion(0)]
	public CommandExpectedContext commandExpected() {
		CommandExpectedContext _localctx = new CommandExpectedContext(Context, State);
		EnterRule(_localctx, 12, RULE_commandExpected);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 84;
			Match(T__6);
			State = 85;
			Match(TEXT);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class StopExpectedContext : ParserRuleContext {
		public StopExpectedContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_stopExpected; } }
	}

	[RuleVersion(0)]
	public StopExpectedContext stopExpected() {
		StopExpectedContext _localctx = new StopExpectedContext(Context, State);
		EnterRule(_localctx, 14, RULE_stopExpected);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 87;
			Match(T__7);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ActionSelectContext : ParserRuleContext {
		public IToken option;
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMBER() { return GetToken(YarnSpinnerTestPlanParser.NUMBER, 0); }
		public ActionSelectContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_actionSelect; } }
	}

	[RuleVersion(0)]
	public ActionSelectContext actionSelect() {
		ActionSelectContext _localctx = new ActionSelectContext(Context, State);
		EnterRule(_localctx, 16, RULE_actionSelect);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 89;
			Match(T__8);
			State = 90;
			_localctx.option = Match(NUMBER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ActionSetContext : ParserRuleContext {
		public ActionSetContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_actionSet; } }
	 
		public ActionSetContext() { }
		public virtual void CopyFrom(ActionSetContext context) {
			base.CopyFrom(context);
		}
	}
	public partial class ActionSetBoolContext : ActionSetContext {
		public IToken variable;
		public IToken value;
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode VARIABLE() { return GetToken(YarnSpinnerTestPlanParser.VARIABLE, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode BOOL() { return GetToken(YarnSpinnerTestPlanParser.BOOL, 0); }
		public ActionSetBoolContext(ActionSetContext context) { CopyFrom(context); }
	}
	public partial class ActionSetNumberContext : ActionSetContext {
		public IToken variable;
		public IToken value;
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode VARIABLE() { return GetToken(YarnSpinnerTestPlanParser.VARIABLE, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NUMBER() { return GetToken(YarnSpinnerTestPlanParser.NUMBER, 0); }
		public ActionSetNumberContext(ActionSetContext context) { CopyFrom(context); }
	}

	[RuleVersion(0)]
	public ActionSetContext actionSet() {
		ActionSetContext _localctx = new ActionSetContext(Context, State);
		EnterRule(_localctx, 18, RULE_actionSet);
		try {
			State = 100;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,9,Context) ) {
			case 1:
				_localctx = new ActionSetBoolContext(_localctx);
				EnterOuterAlt(_localctx, 1);
				{
				State = 92;
				Match(T__9);
				State = 93;
				((ActionSetBoolContext)_localctx).variable = Match(VARIABLE);
				State = 94;
				Match(T__10);
				State = 95;
				((ActionSetBoolContext)_localctx).value = Match(BOOL);
				}
				break;
			case 2:
				_localctx = new ActionSetNumberContext(_localctx);
				EnterOuterAlt(_localctx, 2);
				{
				State = 96;
				Match(T__9);
				State = 97;
				((ActionSetNumberContext)_localctx).variable = Match(VARIABLE);
				State = 98;
				Match(T__10);
				State = 99;
				((ActionSetNumberContext)_localctx).value = Match(NUMBER);
				}
				break;
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ActionSetSaliencyModeContext : ParserRuleContext {
		public IToken saliencyMode;
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER() { return GetToken(YarnSpinnerTestPlanParser.IDENTIFIER, 0); }
		public ActionSetSaliencyModeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_actionSetSaliencyMode; } }
	}

	[RuleVersion(0)]
	public ActionSetSaliencyModeContext actionSetSaliencyMode() {
		ActionSetSaliencyModeContext _localctx = new ActionSetSaliencyModeContext(Context, State);
		EnterRule(_localctx, 20, RULE_actionSetSaliencyMode);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 102;
			Match(T__11);
			State = 103;
			_localctx.saliencyMode = Match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ActionJumpToNodeContext : ParserRuleContext {
		public IToken nodeName;
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER() { return GetToken(YarnSpinnerTestPlanParser.IDENTIFIER, 0); }
		public ActionJumpToNodeContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_actionJumpToNode; } }
	}

	[RuleVersion(0)]
	public ActionJumpToNodeContext actionJumpToNode() {
		ActionJumpToNodeContext _localctx = new ActionJumpToNodeContext(Context, State);
		EnterRule(_localctx, 22, RULE_actionJumpToNode);
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 105;
			Match(T__12);
			State = 106;
			_localctx.nodeName = Match(IDENTIFIER);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,20,109,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,2,7,
		7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,1,0,1,0,1,0,5,0,28,8,0,10,0,12,
		0,31,9,0,1,0,1,0,1,1,4,1,36,8,1,11,1,12,1,37,1,2,1,2,1,2,1,2,1,2,1,2,1,
		2,1,2,3,2,48,8,2,1,3,1,3,4,3,52,8,3,11,3,12,3,53,1,4,1,4,1,4,5,4,59,8,
		4,10,4,12,4,62,9,4,1,4,1,4,1,4,5,4,67,8,4,10,4,12,4,70,9,4,3,4,72,8,4,
		1,5,1,5,1,5,5,5,77,8,5,10,5,12,5,80,9,5,1,5,3,5,83,8,5,1,6,1,6,1,6,1,7,
		1,7,1,8,1,8,1,8,1,9,1,9,1,9,1,9,1,9,1,9,1,9,1,9,3,9,101,8,9,1,10,1,10,
		1,10,1,11,1,11,1,11,1,11,1,53,0,12,0,2,4,6,8,10,12,14,16,18,20,22,0,0,
		112,0,24,1,0,0,0,2,35,1,0,0,0,4,47,1,0,0,0,6,49,1,0,0,0,8,71,1,0,0,0,10,
		73,1,0,0,0,12,84,1,0,0,0,14,87,1,0,0,0,16,89,1,0,0,0,18,100,1,0,0,0,20,
		102,1,0,0,0,22,105,1,0,0,0,24,29,3,2,1,0,25,26,5,1,0,0,26,28,3,2,1,0,27,
		25,1,0,0,0,28,31,1,0,0,0,29,27,1,0,0,0,29,30,1,0,0,0,30,32,1,0,0,0,31,
		29,1,0,0,0,32,33,5,0,0,1,33,1,1,0,0,0,34,36,3,4,2,0,35,34,1,0,0,0,36,37,
		1,0,0,0,37,35,1,0,0,0,37,38,1,0,0,0,38,3,1,0,0,0,39,48,3,8,4,0,40,48,3,
		10,5,0,41,48,3,12,6,0,42,48,3,14,7,0,43,48,3,16,8,0,44,48,3,18,9,0,45,
		48,3,22,11,0,46,48,3,20,10,0,47,39,1,0,0,0,47,40,1,0,0,0,47,41,1,0,0,0,
		47,42,1,0,0,0,47,43,1,0,0,0,47,44,1,0,0,0,47,45,1,0,0,0,47,46,1,0,0,0,
		48,5,1,0,0,0,49,51,5,2,0,0,50,52,9,0,0,0,51,50,1,0,0,0,52,53,1,0,0,0,53,
		54,1,0,0,0,53,51,1,0,0,0,54,7,1,0,0,0,55,56,5,3,0,0,56,60,5,20,0,0,57,
		59,3,6,3,0,58,57,1,0,0,0,59,62,1,0,0,0,60,58,1,0,0,0,60,61,1,0,0,0,61,
		72,1,0,0,0,62,60,1,0,0,0,63,64,5,3,0,0,64,68,5,4,0,0,65,67,3,6,3,0,66,
		65,1,0,0,0,67,70,1,0,0,0,68,66,1,0,0,0,68,69,1,0,0,0,69,72,1,0,0,0,70,
		68,1,0,0,0,71,55,1,0,0,0,71,63,1,0,0,0,72,9,1,0,0,0,73,74,5,5,0,0,74,78,
		5,20,0,0,75,77,3,6,3,0,76,75,1,0,0,0,77,80,1,0,0,0,78,76,1,0,0,0,78,79,
		1,0,0,0,79,82,1,0,0,0,80,78,1,0,0,0,81,83,5,6,0,0,82,81,1,0,0,0,82,83,
		1,0,0,0,83,11,1,0,0,0,84,85,5,7,0,0,85,86,5,20,0,0,86,13,1,0,0,0,87,88,
		5,8,0,0,88,15,1,0,0,0,89,90,5,9,0,0,90,91,5,19,0,0,91,17,1,0,0,0,92,93,
		5,10,0,0,93,94,5,18,0,0,94,95,5,11,0,0,95,101,5,16,0,0,96,97,5,10,0,0,
		97,98,5,18,0,0,98,99,5,11,0,0,99,101,5,19,0,0,100,92,1,0,0,0,100,96,1,
		0,0,0,101,19,1,0,0,0,102,103,5,12,0,0,103,104,5,17,0,0,104,21,1,0,0,0,
		105,106,5,13,0,0,106,107,5,17,0,0,107,23,1,0,0,0,10,29,37,47,53,60,68,
		71,78,82,100
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace Yarn.Compiler
