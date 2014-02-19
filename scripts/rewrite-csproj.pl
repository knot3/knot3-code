#!/usr/bin/perl

use strict;
use warnings;

sub find_cs_files {
	my ($dir, $exclude) = @_;

	return
		grep { my $file = $_; scalar (grep { $file =~ /$_/i } @$exclude) ? 0 : 1 }
		map { s/$dir//gm; $_ } split(/[\r\n]+/, `find $dir -name "*.cs"`);
}

sub format_cs_files {
	my (@files) = @_;
	return map { s/\//\\/gm; '    <Compile Include="'.$_.'" />' } sort @files;
}

sub rewrite_csproj {
	my ($args) = @_;

	open my $f, '<', $args->{csproj};
	my @lines = <$f>;
	map { tr/[\r\n]//d; } @lines;
	close $f;

	my @newlines = ();

	while (my $line = shift @lines) {
		last if $line =~ /Compile Include/;
		push @newlines, $line;
	}
	while (my $line = shift @lines) {
		if ($line !~ /Compile Include/) {
			unshift @lines, $line;
			last;
		}
	}
	push @newlines, format_cs_files(find_cs_files($args->{dir}, $args->{exclude}));
	while (my $line = shift @lines) {
		next if $line =~ /Compile Include/;
		push @newlines, $line;
	}

	open $f, '>', $args->{csproj};
	print $f join($args->{linesep}, @newlines, "");
	close $f;
}

my @csproj_files = (
	{ csproj => 'src/Knot3.Game-MonoGame.csproj', dir => 'src/', exclude => ['-XNA.cs'], linesep => qq[\n] },
	{ csproj => 'src/Knot3.Game-XNA.csproj', dir => 'src/', exclude => ['-MonoGame.cs'], linesep => qq[\n] },
	{ csproj => 'framework/Knot3.Framework-MonoGame.csproj', dir => 'framework/', exclude => ['-XNA.cs'], linesep => qq[\n] },
	{ csproj => 'framework/Knot3.Framework-XNA.csproj', dir => 'framework/', exclude => ['-MonoGame.cs'], linesep => qq[\n] },
	{ csproj => 'tests/Knot3.UnitTests-MonoGame.csproj', dir => 'tests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'tests/Knot3.UnitTests-XNA.csproj', dir => 'tests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'tools/ConfigReset/Knot3.ConfigReset.csproj', dir => 'tools/ConfigReset/', exclude => [], linesep => qq[\n] },
	{ csproj => 'extremetests/Knot3.ExtremeTests-MonoGame.csproj', dir => 'extremetests/', exclude => [], linesep => qq[\n] },
	{ csproj => 'extremetests/Knot3.ExtremeTests-XNA.csproj', dir => 'extremetests/', exclude => [], linesep => qq[\n] },
);

foreach my $csproj_file (@csproj_files) {
	rewrite_csproj($csproj_file);
}
