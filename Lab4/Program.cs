// Lab 4
// Student name: Rihem Sayadi
// Student number: 26366518

using System;
using System.Collections.Generic;

Console.WriteLine("CPEN223 Lab 4");

// Testing: Write test cases that exercise all four methods you are to implement.

Dictionary<string, int> counts = GenomeAnalyzer.CountKMers("AAAA", 2);
Console.WriteLine(
    $"Expected: AA -> 3, Actual: AA -> {counts["AA"]} ({counts.Count} entries)");

Dictionary<string, int> changes =
    GenomeAnalyzer.CompareProfiles("AAAA", "TTTT", 2);

Console.WriteLine(
    $"Expected: AA -> -3, Actual: AA -> {changes["AA"]}");

Console.WriteLine(
    $"Expected: TT -> 3, Actual: TT -> {changes["TT"]}");

List<string> mostChanged =
    GenomeAnalyzer.MostChangedKMers("AAAA", "TTTT", 2);

Console.WriteLine("Expected: AA and TT");
Console.WriteLine("Actual:");

foreach (string kmer in mostChanged)
{
    Console.WriteLine(kmer);
}

bool differ =
    GenomeAnalyzer.SamplesDiffer("AAAA", "TTTT", 2, 3);

Console.WriteLine(
    $"Expected: True, Actual: {differ}");


// end Testing code

// Do not change the program skeleton: keep the class name, method names,
// parameters, and return types exactly as given.
// Do not use LINQ, and do not use Console inside the GenomeAnalyzer methods.

public static class GenomeAnalyzer
{
    public static Dictionary<string, int> CountKMers(string sequence, int k)
    {
        ValidateSequence(sequence);

        if (k <= 0)
        {
            throw new ArgumentException("k must be positive");
        }

        if (k > sequence.Length)
        {
            throw new ArgumentException(
                "k cannot be greater than the sequence length");
        }

        Dictionary<string, int> profile =
            new Dictionary<string, int>();

        // Move one position at a time so overlapping k-mers are counted.
        for (int i = 0; i <= sequence.Length - k; i++)
        {
            string kmer = sequence.Substring(i, k);

            if (profile.ContainsKey(kmer))
            {
                profile[kmer]++;
            }
            else
            {
                profile[kmer] = 1;
            }
        }

        return profile;
    }

    public static Dictionary<string, int> CompareProfiles(
        string reference, string sample, int k)
    {
        ValidateComparisonArguments(reference, sample, k);

        Dictionary<string, int> referenceProfile =
            CountKMers(reference, k);

        Dictionary<string, int> sampleProfile =
            CountKMers(sample, k);

        Dictionary<string, int> changes =
            new Dictionary<string, int>();

        foreach (KeyValuePair<string, int> pair in referenceProfile)
        {
            string kmer = pair.Key;
            int sampleCount = 0;

            if (sampleProfile.ContainsKey(kmer))
            {
                sampleCount = sampleProfile[kmer];
            }

            int change = sampleCount - pair.Value;

            if (change != 0)
            {
                changes[kmer] = change;
            }
        }

        // Add k-mers that occur only in the sample.
        foreach (KeyValuePair<string, int> pair in sampleProfile)
        {
            if (!referenceProfile.ContainsKey(pair.Key))
            {
                changes[pair.Key] = pair.Value;
            }
        }

        return changes;
    }

    public static List<string> MostChangedKMers(
        string reference, string sample, int k)
    {
        Dictionary<string, int> changes =
            CompareProfiles(reference, sample, k);

        List<string> result = new List<string>();
        int largestChange = 0;

        foreach (KeyValuePair<string, int> pair in changes)
        {
            int absoluteChange = Math.Abs(pair.Value);

            if (absoluteChange > largestChange)
            {
                largestChange = absoluteChange;
            }
        }

        // Include every k-mer tied for the largest absolute change.
        foreach (KeyValuePair<string, int> pair in changes)
        {
            if (Math.Abs(pair.Value) == largestChange)
            {
                result.Add(pair.Key);
            }
        }

        return result;
    }

    public static bool SamplesDiffer(
        string reference, string sample, int k, int threshold)
    {
        if (threshold <= 0)
        {
            throw new ArgumentException(
                "threshold must be positive");
        }

        Dictionary<string, int> changes =
            CompareProfiles(reference, sample, k);

        foreach (KeyValuePair<string, int> pair in changes)
        {
            if (Math.Abs(pair.Value) >= threshold)
            {
                return true;
            }
        }

        return false;
    }

    private static void ValidateComparisonArguments(
        string reference, string sample, int k)
    {
        ValidateSequence(reference);
        ValidateSequence(sample);

        if (k <= 0)
        {
            throw new ArgumentException("k must be positive");
        }

        if (k > reference.Length || k > sample.Length)
        {
            throw new ArgumentException(
                "k cannot be greater than either sequence length");
        }
    }

    private static void ValidateSequence(string sequence)
    {
        if (sequence == null)
        {
            throw new ArgumentException(
                "Sequence cannot be null");
        }

        foreach (char nucleotide in sequence)
        {
            if (nucleotide != 'A' &&
                nucleotide != 'C' &&
                nucleotide != 'G' &&
                nucleotide != 'T')
            {
                throw new ArgumentException(
                    "Sequence must contain only A, C, G, and T");
            }
        }
    }
}